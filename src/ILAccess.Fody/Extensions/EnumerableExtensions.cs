namespace ILAccess.Fody.Extensions;

public static class EnumerableExtensions
{
    public static string JoinWith<T>(this IEnumerable<T> enumerable, string? separator)
    {
        return string.Join(separator, enumerable);
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
    {
        return source ?? Enumerable.Empty<T>();
    }
}