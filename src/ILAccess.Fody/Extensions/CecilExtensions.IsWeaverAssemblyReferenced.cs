namespace ILAccess.Fody.Extensions;

internal static partial class CecilExtensions
{
    private static readonly ConcurrentDictionary<TypeReference, bool> _usageCache = new();

    public static bool IsWeaverAssemblyReferenced(this TypeReference? type, ModuleDefinition module)
    {
        if (type == null)
            return false;

        return _usageCache.GetOrAdd(type, k => DoCheck(k, module));

        static bool DoCheck(TypeReference typeRef, ModuleDefinition module)
        {
            return typeRef switch
            {
                GenericInstanceType t => t.ElementType.IsWeaverAssemblyReferenced(module)
                                         || t.GenericParameters.Any(i => i.IsWeaverAssemblyReferenced(module))
                                         || t.GenericArguments.Any(i => i.IsWeaverAssemblyReferenced(module)),
                GenericParameter t => t.HasConstraints && t.Constraints.Any(c => c.IsWeaverAssemblyReferenced(module))
                                      || t.HasCustomAttributes && t.CustomAttributes.Any(i => i.IsWeaverAssemblyReferenced(module)),
                IModifierType t => t.ElementType.IsWeaverAssemblyReferenced(module) || t.ModifierType.IsWeaverAssemblyReferenced(module),
                FunctionPointerType t => ((IMethodSignature)t).IsWeaverAssemblyReferenced(module),
                _ => typeRef.Scope is { MetadataScopeType: MetadataScopeType.AssemblyNameReference, Name: WeaverAnchors.AssemblyName }
            };
        }
    }

    public static bool IsWeaverAssemblyReferenced(this IMethodSignature? method, ModuleDefinition module)
    {
        if (method == null)
            return false;

        if (method.ReturnType.IsWeaverAssemblyReferenced(module) || method.HasParameters && method.Parameters.Any(i => i.IsWeaverAssemblyReferenced(module)))
            return true;

        if (method is IGenericInstance { HasGenericArguments: true } genericInstance && genericInstance.GenericArguments.Any(i => i.IsWeaverAssemblyReferenced(module)))
            return true;

        if (method is IGenericParameterProvider { HasGenericParameters: true } generic && generic.GenericParameters.Any(i => i.IsWeaverAssemblyReferenced(module)))
            return true;

        if (method is MethodReference methodRef)
        {
            if (methodRef is MethodDefinition methodDef)
            {
                if (methodDef.HasCustomAttributes && methodDef.CustomAttributes.Any(i => i.IsWeaverAssemblyReferenced(module)))
                    return true;
            }
            else
            {
                if (methodRef.DeclaringType.IsWeaverAssemblyReferenced(module))
                    return true;
            }
        }

        return false;
    }

    public static bool IsWeaverAssemblyReferenced(this FieldReference? fieldRef, ModuleDefinition module)
    {
        if (fieldRef == null)
            return false;

        if (fieldRef.FieldType.IsWeaverAssemblyReferenced(module))
            return true;

        if (fieldRef is FieldDefinition fieldDef)
        {
            if (fieldDef.HasCustomAttributes && fieldDef.CustomAttributes.Any(i => i.IsWeaverAssemblyReferenced(module)))
                return true;
        }
        else
        {
            if (fieldRef.DeclaringType.IsWeaverAssemblyReferenced(module))
                return true;
        }

        return false;
    }

    private static bool IsWeaverAssemblyReferenced(this ParameterDefinition? paramDef, ModuleDefinition module)
    {
        if (paramDef == null)
            return false;

        if (paramDef.ParameterType.IsWeaverAssemblyReferenced(module))
            return true;

        if (paramDef.HasCustomAttributes && paramDef.CustomAttributes.Any(i => i.IsWeaverAssemblyReferenced(module)))
            return true;

        return false;
    }

    private static bool IsWeaverAssemblyReferenced(this CustomAttribute? attr, ModuleDefinition module)
    {
        if (attr == null)
            return false;

        if (attr.AttributeType.IsWeaverAssemblyReferenced(module))
            return true;

        if (attr.HasConstructorArguments && attr.ConstructorArguments.Any(i => i.Value is TypeReference typeRef && typeRef.IsWeaverAssemblyReferenced(module)))
            return true;

        if (attr.HasProperties && attr.Properties.Any(i => i.Argument.Value is TypeReference typeRef && typeRef.IsWeaverAssemblyReferenced(module)))
            return true;

        return false;
    }

    private static bool IsWeaverAssemblyReferenced(this GenericParameterConstraint? constraint, ModuleDefinition module)
    {
        if (constraint == null)
            return false;

        if (constraint.ConstraintType.IsWeaverAssemblyReferenced(module))
            return true;

        if (constraint.HasCustomAttributes && constraint.CustomAttributes.Any(i => i.IsWeaverAssemblyReferenced(module)))
            return true;

        return false;
    }
}
