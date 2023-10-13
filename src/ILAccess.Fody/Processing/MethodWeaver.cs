using ILAccess.Fody.Extensions;
using Mono.Collections.Generic;

namespace ILAccess.Fody.Processing;

internal sealed class MethodWeaver
{
    private readonly ModuleDefinition _module;
    private readonly MethodDefinition _method;
    private readonly MethodWeaverLogger _log;
    private readonly WeaverILProcessor _il;
    private readonly References _references;
    private Collection<Instruction> Instructions => _method.Body.Instructions;
    private TypeReferences Types => _references.Types;
    private MethodReferences Methods => _references.Methods;

    public MethodWeaver(ModuleDefinition module, MethodDefinition method, ILogger log)
    {
        _module = module;
        _method = method;
        _il = new WeaverILProcessor(method);
        _log = new MethodWeaverLogger(log, _method);
        _references = new References(module);
    }

    public static bool HasLibReference(ModuleDefinition module, MethodDefinition method)
    {
        if (method.IsWeaverAssemblyReferenced(module))
            return true;

        if (!method.HasBody)
            return false;

        if (method.Body.HasVariables && method.Body.Variables.Any(i => i.VariableType.IsWeaverAssemblyReferenced(module)))
            return true;

        foreach (var instruction in method.Body.Instructions)
        {
            switch (instruction.Operand)
            {
                case MethodReference methodRef when methodRef.IsWeaverAssemblyReferenced(module):
                case TypeReference typeRef when typeRef.IsWeaverAssemblyReferenced(module):
                case FieldReference fieldRef when fieldRef.IsWeaverAssemblyReferenced(module):
                case CallSite callSite when callSite.IsWeaverAssemblyReferenced(module):
                    return true;
            }
        }

        return false;
    }

    public void Process()
    {
        try
        {
            ProcessImpl();
        }
        catch (InstructionWeavingException ex)
        {
            throw new WeavingException(_log.QualifyMessage(ex.Message, ex.Instruction))
            {
                SequencePoint = ex.Instruction.GetInputSequencePoint(_method)
            };
        }
        catch (WeavingException ex)
        {
            throw new WeavingException(_log.QualifyMessage(ex.Message))
            {
                SequencePoint = ex.SequencePoint
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error occured while processing method {_method.FullName}: {ex.Message}", ex);
        }
    }

    private static bool IsAnchorMethodCall(Instruction instruction)
    {
        return instruction.OpCode == OpCodes.Call
               && instruction.Operand is MethodReference
               {
                   Name: WeaverAnchors.MethodName,
                   DeclaringType.Scope.Name: WeaverAnchors.AssemblyName
               };
    }

    private void ProcessImpl()
    {
        var instruction = Instructions.FirstOrDefault();
        Instruction? nextInstruction;

        for (; instruction != null; instruction = nextInstruction)
        {
            nextInstruction = instruction.Next;

            if (IsAnchorMethodCall(instruction) == false)
                continue;

            try
            {
                nextInstruction = ProcessAnchorMethod(instruction);
            }
            catch (InstructionWeavingException)
            {
                throw;
            }
            catch (WeavingException ex)
            {
                throw new InstructionWeavingException(instruction, _log.QualifyMessage(ex.Message, instruction));
            }
            catch (Exception ex)
            {
                throw new InstructionWeavingException(instruction, $"Unexpected error occured while processing method {_method.FullName} at instruction {instruction}: {ex}");
            }
        }
    }

    private Instruction? ProcessAnchorMethod(Instruction instruction)
    {
        var anchorMethod = (MethodReference)instruction.Operand;

        var next = instruction.Next;
        if (next == null)
            throw NoILAccessorMethodInvocationException();

        // var x = obj.ILAccess<T>();
        if (IsStloc(next))
            throw NoILAccessorMethodInvocationException();

        // obj.Base<T>().SomeMethod();
        if (IsCallAndPop(next) && IsILAccessorMethod(next) == false)
        {
            if (IsAnchorMethodCall(next))
                throw new InvalidOperationException($"The method {WeaverAnchors.MethodCall} cannot be invoked followed by another {WeaverAnchors.MethodCall}");
            else
                throw NoILAccessorMethodInvocationException();
        }

        for (var p = next; p != null; p = p.Next)
        {
            if (IsILAccessorMethod(p) == false)
                continue;

            p = EmitILAccessInstructions(instruction, p);
            return p.Next;
        }

        throw NoILAccessorMethodInvocationException();
    }

    private Instruction EmitILAccessInstructions(Instruction anchor, Instruction invokeInstruction)
    {
        _il.Remove(anchor);

        var instructions = new List<Instruction>(4)
        {

        };

        var cur = _il.InsertAfter(invokeInstruction, instructions);
        _il.Remove(invokeInstruction);
        return cur;
    }

    private static bool IsILAccessorMethod(Instruction instruction)
    {
        return instruction.OpCode == OpCodes.Callvirt
               && instruction.Operand is MethodReference
               {
                   DeclaringType:
                   {
                       Name: WeaverAnchors.TypeGenericName,
                       Scope.Name: WeaverAnchors.AssemblyName
                   }
               };
    }

    private static bool IsCallAndPop(Instruction instruction)
    {
        return instruction.OpCode.FlowControl == FlowControl.Call && instruction.GetPopCount() > 0;
    }

    private static bool IsStloc(Instruction instruction)
    {
        switch (instruction.OpCode.Code)
        {
            case Code.Stloc:
            case Code.Stloc_S:
            case Code.Stloc_0:
            case Code.Stloc_1:
            case Code.Stloc_2:
            case Code.Stloc_3:
                return true;
            default:
                return false;
        }
    }

    private static Exception NoILAccessorMethodInvocationException()
    {
        return new InvalidOperationException($"The method {WeaverAnchors.MethodCall} requires one method declared in {WeaverAnchors.TypeName}<T> to be invoked fluently");
    }

    private static void EnsureNonAbstract(MethodDefinition method)
    {
        if (method.IsAbstract)
            throw new InvalidOperationException($"The abstract interface method {method.FullName} cannot be invoked");
    }
}
