namespace ILAccess.Fody.Extensions;

public static class PropertyDefinitionExtensions
{
    public static MethodReference BuildSetter(this PropertyDefinition property)
    {
        if (property.SetMethod == null)
            throw new WeavingException($"Property '{property.Name}' in type {property.DeclaringType.FullName} has no setter");

        return new MethodRefBuilder(property.Module, property.DeclaringType, property.SetMethod).Build();
    }

    public static MethodReference BuildGetter(this PropertyDefinition property)
    {
        if (property.GetMethod == null)
            throw new WeavingException($"Property '{property.Name}' in type {property.DeclaringType.FullName} has no getter");

        return new MethodRefBuilder(property.Module, property.DeclaringType, property.GetMethod).Build();
    }
}