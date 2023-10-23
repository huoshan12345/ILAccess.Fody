using System.Diagnostics.CodeAnalysis;

namespace ILAccess;

/// <summary>
/// 
/// </summary>
[SuppressMessage("ReSharper", "UnusedParameter.Global")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
public static class ObjectExtension
{
    [DoesNotReturn]
    private static IILAccessor Throw()
    {
        throw new InvalidOperationException("This method is meant to be replaced at compile time by ILAccess.Fody, but the weaver has not been executed correctly.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static IILAccessor ILAccess<T>(this T? instance) => Throw();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IILAccessor ILAccess(this object? instance, Type type) => Throw();
}
