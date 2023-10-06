using System;

namespace ILAccess;

/// <summary>
/// 
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static TInterface Base<TInterface>(this TInterface instance) =>
        throw new InvalidOperationException("This method is meant to be replaced at compile time by ILAccess.Fody, but the weaver has not been executed correctly.");

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static ILAccessor<T> ILAccess<T>(this T? instance) => new(instance);
}
