namespace ILAccess.Fody.Extensions;

public static class TypeReferenceExtensions
{
    /// <summary>
    /// Get name of type with generic parameters without namespace.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string SimpleName(this TypeReference type)
    {
        if (type.IsGenericInstance == false) 
            return type.Name;

        var name = type.Name;
        var index = name.IndexOf('`');
        return index == -1 ? name : name.Substring(0, index);
    }
}