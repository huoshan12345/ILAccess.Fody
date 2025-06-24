using MoreFodyHelpers.Support;
using static ILAccess.Fody.Processing.WeaverAnchors.MethodNames;

namespace ILAccess.Fody.Processing;

internal sealed class MethodWeaver
{
    private readonly ModuleDefinition _module;
    private readonly ModuleWeavingContext _context;
    private readonly MethodDefinition _method;
    private readonly MethodWeaverLogger _log;
    private readonly WeaverILProcessor _il;
    private readonly SequencePointMapper _sequencePoints;
    private IEnumerable<Instruction> Instructions => _method.Body.Instructions;

    public MethodWeaver(ModuleWeavingContext context, ModuleDefinition module, MethodDefinition method, IWeaverLogger log)
    {
        _context = context;
        _module = module;
        _method = method;
        _il = new WeaverILProcessor(method);
        _sequencePoints = new SequencePointMapper(method, true);
        _log = new MethodWeaverLogger(log, _method);
    }

    public bool Process()
    {
        try
        {
            return ProcessImpl();
        }
        catch (InstructionWeavingException ex)
        {
            throw new WeavingException(_log.QualifyMessage(ex.Message, ex.Instruction))
            {
                SequencePoint = _sequencePoints.GetInputSequencePoint(ex.Instruction),
            };
        }
        catch (WeavingException ex)
        {
            throw new WeavingException(_log.QualifyMessage(ex.Message))
            {
                SequencePoint = ex.SequencePoint,
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

    private bool ProcessImpl()
    {
        var instruction = Instructions.FirstOrDefault();
        Instruction? nextInstruction;
        var emitted = false;

        for (; instruction != null; instruction = nextInstruction)
        {
            nextInstruction = instruction.Next;

            if (IsAnchorMethodCall(instruction) == false)
                continue;

            try
            {
                nextInstruction = ProcessAnchorMethod(instruction);
                emitted = true;
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
        return emitted;
    }

    private Instruction? ProcessAnchorMethod(Instruction instruction)
    {
        var next = instruction.Next;
        if (next == null)
            throw NoILAccessorMethodInvocationException();

        // var x = obj.ILAccess<T>();
        if (IsStloc(next))
            throw NoILAccessorMethodInvocationException();

        // obj.ILAccess<T>().SomeMethod();
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

    private Instruction EmitPropertyInstructions(Instruction anchor, PropertyDefinition property, bool isGet)
    {
        if (isGet)
        {
            var method = property.BuildGetter(_context).Resolve();
            if (method.IsStatic)
            {
                // call         void ILAccess.Example.TestModel::set_PublicStaticProperty(int32)
                _il.Remove(anchor.Previous);
                return Instruction.Create(OpCodes.Call, method);
            }
            else
            {
                // ldloc.0      // obj
                // callvirt instance void ILAccess.Example.TestModel::set_PublicProperty(int32)
                return Instruction.Create(OpCodes.Callvirt, method);
            }
        }
        else
        {
            var method = property.BuildSetter(_context).Resolve();
            if (method.IsStatic)
            {
                // call         void ILAccess.Example.TestModel::set_PublicStaticProperty(int32)
                _il.Remove(anchor.Previous);
                return Instruction.Create(OpCodes.Call, method);
            }
            else
            {
                // ldloc.0      // obj
                // callvirt instance void ILAccess.Example.TestModel::set_PublicProperty(int32)
                return Instruction.Create(OpCodes.Callvirt, method);
            }
        }
    }

    private Instruction EmitFieldInstructions(Instruction anchor, FieldDefinition field, bool isGet)
    {
        if (field.IsStatic)
        {
            _il.Remove(anchor.Previous);
        }

        var code = (field.IsStatic, isGet) switch
        {
            (true, true) => OpCodes.Ldsfld,
            (true, false) => OpCodes.Stsfld,
            (false, true) => OpCodes.Ldfld,
            (false, false) => OpCodes.Stfld,
        };
        return Instruction.Create(code, field);
    }

    private Instruction EmitILAccessInstructions(Instruction anchor, Instruction invokeInstruction)
    {
        var next = anchor.Next;
        if (next.OpCode != OpCodes.Ldstr)
            throw new InvalidOperationException("The name of property/field to be invoked should be a constant.");

        var typeRef = ((GenericInstanceMethod)anchor.Operand).GenericArguments[0];
        var name = (string)next.Operand;
        var method = (MethodReference)invokeInstruction.Operand;
        var typeDef = typeRef.ResolveRequiredType(_context);
        var properties = typeDef.Properties.Where(p => p.Name == name).ToArray();
        var fields = typeDef.Fields.Where(p => p.Name == name).ToArray();
        var isGet = method.Name == GetValue;

        var newInstruction = (properties.Length, fields.Length) switch
        {
            (0, 0) => throw new InvalidOperationException($"Property or field '{name}' not found in type {typeDef.FullName}"),
            (1, 0) => EmitPropertyInstructions(anchor, properties[0], isGet),
            (0, 1) => EmitFieldInstructions(anchor, fields[0], isGet),
            _ => throw new InvalidOperationException($"Ambiguous property or field '{name}' in type {typeDef.FullName}"),
        };

        _il.Remove(anchor);
        _il.Remove(next);
        _il.Replace(invokeInstruction, newInstruction);
        return newInstruction;
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
}
