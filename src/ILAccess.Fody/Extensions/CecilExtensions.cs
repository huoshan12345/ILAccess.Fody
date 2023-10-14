namespace ILAccess.Fody.Extensions;

internal static partial class CecilExtensions
{
    public static TypeDefinition ResolveRequiredType(this TypeReference typeRef)
    {
        TypeDefinition typeDef;

        try
        {
            typeDef = typeRef.Resolve();
        }
        catch (Exception ex)
        {
            throw new WeavingException($"Could not resolve type {typeRef.FullName}: {ex.Message}");
        }

        return typeDef ?? throw new WeavingException($"Could not resolve type {typeRef.FullName}");
    }

    public static bool IsForwardedType(this ExportedType exportedType)
    {
        for (; exportedType != null; exportedType = exportedType.DeclaringType)
        {
            if (exportedType.IsForwarder)
                return true;
        }

        return false;
    }

    private static TypeDefinition ResolveRequiredType(this ExportedType exportedType)
    {
        TypeDefinition typeDef;

        try
        {
            typeDef = exportedType.Resolve();
        }
        catch (Exception ex)
        {
            throw new WeavingException($"Could not resolve type {exportedType.FullName}: {ex.Message}");
        }

        return typeDef ?? throw new WeavingException($"Could not resolve type {exportedType.FullName}");
    }

    public static TypeReference Clone(this TypeReference typeRef)
    {
        var clone = new TypeReference(typeRef.Namespace, typeRef.Name, typeRef.Module, typeRef.Scope, typeRef.IsValueType)
        {
            DeclaringType = typeRef.DeclaringType
        };

        if (typeRef.HasGenericParameters)
        {
            foreach (var param in typeRef.GenericParameters)
                clone.GenericParameters.Add(new GenericParameter(param.Name, clone));
        }

        return clone;
    }

    public static TypeReference CreateReference(this ExportedType exportedType, ModuleDefinition exportingModule, ModuleDefinition targetModule)
    {
        var typeDef = exportedType.ResolveRequiredType();
        var metadataScope = MapAssemblyReference(targetModule, exportingModule.Assembly.Name);

        var typeRef = new TypeReference(exportedType.Namespace, exportedType.Name, exportingModule, metadataScope, typeDef.IsValueType)
        {
            DeclaringType = exportedType.DeclaringType?.CreateReference(exportingModule, targetModule)
        };

        if (typeDef.HasGenericParameters)
        {
            foreach (var param in typeDef.GenericParameters)
                typeRef.GenericParameters.Add(new GenericParameter(param.Name, typeRef));
        }

        return typeRef;
    }

    private static AssemblyNameReference MapAssemblyReference(ModuleDefinition module, AssemblyNameReference name)
    {
        // Try to map to an existing assembly reference by name,
        // to avoid adding additional versions of a referenced assembly
        // (netstandard v2.0 can be mapped to netstandard 2.1 for instance)

        foreach (var assemblyReference in module.AssemblyReferences)
        {
            if (assemblyReference.Name == name.Name)
                return assemblyReference;
        }

        return name;
    }

    private static TypeReference MapToScope(this TypeReference typeRef, IMetadataScope scope, ModuleDefinition module)
    {
        if (scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference)
        {
            var assemblyName = (AssemblyNameReference)scope;
            var assembly = module.AssemblyResolver.Resolve(assemblyName) ?? throw new WeavingException($"Could not resolve assembly {assemblyName.Name}");

            if (assembly.MainModule.HasExportedTypes)
            {
                foreach (var exportedType in assembly.MainModule.ExportedTypes)
                {
                    if (!exportedType.IsForwardedType())
                        continue;

                    if (exportedType.FullName == typeRef.FullName)
                        return exportedType.CreateReference(assembly.MainModule, module);
                }
            }
        }

        return typeRef;
    }

    public static MethodReference Clone(this MethodReference method)
    {
        var clone = new MethodReference(method.Name, method.ReturnType, method.DeclaringType)
        {
            HasThis = method.HasThis,
            ExplicitThis = method.ExplicitThis,
            CallingConvention = method.CallingConvention
        };

        if (method.HasParameters)
        {
            foreach (var param in method.Parameters)
                clone.Parameters.Add(new ParameterDefinition(param.Name, param.Attributes, param.ParameterType));
        }

        if (method.HasGenericParameters)
        {
            foreach (var param in method.GenericParameters)
                clone.GenericParameters.Add(new GenericParameter(param.Name, clone));
        }

        return clone;
    }

    public static MethodReference MapToScope(this MethodReference method, IMetadataScope scope, ModuleDefinition module)
    {
        var clone = new MethodReference(method.Name, method.ReturnType.MapToScope(scope, module), method.DeclaringType.MapToScope(scope, module))
        {
            HasThis = method.HasThis,
            ExplicitThis = method.ExplicitThis,
            CallingConvention = method.CallingConvention
        };

        if (method.HasParameters)
        {
            foreach (var param in method.Parameters)
                clone.Parameters.Add(new ParameterDefinition(param.Name, param.Attributes, param.ParameterType.MapToScope(scope, module)));
        }

        if (method.HasGenericParameters)
        {
            foreach (var param in method.GenericParameters)
                clone.GenericParameters.Add(new GenericParameter(param.Name, clone));
        }

        return clone;
    }

    public static MethodReference MakeGeneric(this MethodReference method, TypeReference declaringType)
    {
        if (!declaringType.IsGenericInstance || method.DeclaringType.IsGenericInstance)
            return method;

        var genericDeclType = new GenericInstanceType(method.DeclaringType);

        foreach (var argument in ((GenericInstanceType)declaringType).GenericArguments)
            genericDeclType.GenericArguments.Add(argument);

        var result = method.Clone();
        result.DeclaringType = genericDeclType;
        return result;
    }

    private static int GetArgCount(OpCode opCode, IMethodSignature method)
    {
        var argCount = method.Parameters.Count;

        if (method.HasThis && !method.ExplicitThis && opCode.Code != Code.Newobj)
            ++argCount;

        if (opCode.Code == Code.Calli)
            ++argCount;

        return argCount;
    }

    public static int GetPopCount(this Instruction instruction)
    {
        if (instruction.OpCode.FlowControl == FlowControl.Call)
            return GetArgCount(instruction.OpCode, (IMethodSignature)instruction.Operand);

        if (instruction.OpCode == OpCodes.Dup)
            return 0;

        switch (instruction.OpCode.StackBehaviourPop)
        {
            case StackBehaviour.Pop0:
                return 0;

            case StackBehaviour.Varpop:
            case StackBehaviour.Popi:
            case StackBehaviour.Popref:
            case StackBehaviour.Pop1:
                return 1;

            case StackBehaviour.Pop1_pop1:
            case StackBehaviour.Popi_pop1:
            case StackBehaviour.Popi_popi:
            case StackBehaviour.Popi_popi8:
            case StackBehaviour.Popi_popr4:
            case StackBehaviour.Popi_popr8:
            case StackBehaviour.Popref_pop1:
            case StackBehaviour.Popref_popi:
                return 2;

            case StackBehaviour.Popi_popi_popi:
            case StackBehaviour.Popref_popi_popi:
            case StackBehaviour.Popref_popi_popi8:
            case StackBehaviour.Popref_popi_popr4:
            case StackBehaviour.Popref_popi_popr8:
            case StackBehaviour.Popref_popi_popref:
                return 3;

            case StackBehaviour.PopAll:
                throw new InstructionWeavingException(instruction, "Unexpected stack-clearing instruction encountered");

            default:
                throw new InstructionWeavingException(instruction, $"Unexpected stack pop behavior: {instruction.OpCode.StackBehaviourPop}");
        }
    }

    public static IEnumerable<Instruction> GetInstructions(this ExceptionHandler handler)
    {
        if (handler.TryStart != null)
            yield return handler.TryStart;

        if (handler.TryEnd != null)
            yield return handler.TryEnd;

        if (handler.FilterStart != null)
            yield return handler.FilterStart;

        if (handler.HandlerStart != null)
            yield return handler.HandlerStart;

        if (handler.HandlerEnd != null)
            yield return handler.HandlerEnd;
    }

    public static SequencePoint? GetInputSequencePoint(this Instruction? instruction, MethodDefinition method)
    {
        if (instruction == null)
            return null;

        var sequencePoints = method.DebugInformation.HasSequencePoints
            ? method.DebugInformation.SequencePoints
            : Enumerable.Empty<SequencePoint>();

        return sequencePoints.LastOrDefault(sp => sp.Offset <= instruction.Offset);
    }
}
