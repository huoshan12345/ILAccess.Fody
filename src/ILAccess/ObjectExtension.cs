namespace ILAccess;

/// <summary>
/// 
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static ILAccessor<T> ILAccess<T>(this T? instance) => new(instance);
}
