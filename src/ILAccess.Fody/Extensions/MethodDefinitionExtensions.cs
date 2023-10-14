namespace ILAccess.Fody.Extensions;

public static class MethodDefinitionExtensions
{
    public static CustomAttribute AddAttribute<T>(this MethodDefinition method, params Type[] parameters) where T : Attribute
    {
        return AddAttribute(method, method.Module.GetConstructor<T>(parameters));
    }

    public static CustomAttribute AddAttribute(this MethodDefinition method, MethodReference constructor)
    {
        var attribute = new CustomAttribute(constructor);
        method.CustomAttributes.Add(attribute);
        return attribute;
    }

    public static ParameterDefinition AddParameter<T>(this MethodDefinition m, string? name = null)
    {
        return m.AddParameter(typeof(T), name);
    }

    public static ParameterDefinition AddParameter(this MethodDefinition m, Type type, string? name = null)
    {
        if (m.Module == null)
        {
            throw new NullReferenceException("This method has yet to be added to the assembly and doesn't have a module. Please provide a module.");
        }
        return m.AddParameter(m.Module.ImportType(type), name);
    }

    public static ParameterDefinition AddParameter(this MethodDefinition m, TypeReference type, string? name = null)
    {
        var parameter = new ParameterDefinition(type);
        m.Parameters.Add(parameter);

        if (!string.IsNullOrEmpty(name))
        {
            parameter.Name = name;
        }

        return parameter;
    }
}