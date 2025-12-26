namespace ILAccess.Fody.Support;

internal static class Extensions
{
    // NOTE: do not use ValueTuple here.
    // see https://github.com/Fody/Fody/pull/911/files
    public static IEnumerable<Tuple<TFirst, TSecond>> Zip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
    {
        return first.Zip(second, Tuple.Create);
    }
}