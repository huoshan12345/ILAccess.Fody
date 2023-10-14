namespace ILAccess.Fody.Support;

internal class TypeReferenceEqualityComparer : IEqualityComparer<TypeReference>
{
    //private static readonly Type _typeReferenceEqualityComparer = typeof(TypeReference).Assembly.GetRequiredType("Mono.Cecil.TypeReferenceEqualityComparer");
    //public static readonly IEqualityComparer<TypeReference> Instance = _typeReferenceEqualityComparer.New<IEqualityComparer<TypeReference>>();

    //public bool Equals(TypeReference x, TypeReference y)
    //{
    //   return Instance.Equals(x, y);
    //}

    //public int GetHashCode(TypeReference obj)
    //{
    //    return Instance.GetHashCode(obj);
    //}

    public static IEqualityComparer<TypeReference> Instance { get; } = new TypeReferenceEqualityComparer();

    public bool Equals(TypeReference? x, TypeReference? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;

        return x.FullName == y.FullName
            && GetAssemblyName(x.Scope) == GetAssemblyName(y.Scope);
    }

    public int GetHashCode(TypeReference? obj)
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 31 + obj?.FullName?.GetHashCode() ?? 0;
            hash = hash * 31 + GetAssemblyName(obj?.Scope)?.GetHashCode() ?? 0;
            return hash;
        }
    }

    private static string? GetAssemblyName(IMetadataScope? scope)
    {
        return scope switch
        {
            null => null,
            ModuleDefinition md => md.Assembly.FullName,
            _ => scope.ToString(),
        };
    }
}