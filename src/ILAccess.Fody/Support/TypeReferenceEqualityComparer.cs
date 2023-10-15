namespace ILAccess.Fody.Support;

internal class TypeReferenceEqualityComparer : IEqualityComparer<TypeReference>
{
    private static readonly string[] RuntimeNames = { "mscorlib", "System.Runtime", "System.Private.CoreLib", "netstandard" };
    public static IEqualityComparer<TypeReference> Instance { get; } = new TypeReferenceEqualityComparer(false);
    public static IEqualityComparer<TypeReference> IgnoreRuntimeDiffInstance { get; } = new TypeReferenceEqualityComparer(true);

    private readonly bool _ignoreRuntimeDiff;

    public TypeReferenceEqualityComparer(bool ignoreRuntimeDiff)
    {
        _ignoreRuntimeDiff = ignoreRuntimeDiff;
    }

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

    private string? GetAssemblyName(IMetadataScope? scope)
    {
        if (scope == null)
            return null;

        var name = scope.Name;

        return RuntimeNames.Contains(name) && _ignoreRuntimeDiff
            ? "System.Runtime"
            : name;
    }
}