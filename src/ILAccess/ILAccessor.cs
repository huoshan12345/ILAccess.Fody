using System;

namespace ILAccess;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly record struct ILAccessor<T>(T? Value)
{
    private const string Error = "This method is meant to be replaced at compile time by ILAccess.Fody, but the weaver has not been executed correctly.";

    /// <summary>
    /// Gets a value of a property.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public TValue? GetPropertyValue<TValue>(string name) => throw new InvalidOperationException(Error);

    /// <summary>
    /// Sets a value of a property.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public ILAccessor<T> SetPropertyValue<TValue>(string name, TValue? value) => throw new InvalidOperationException(Error);

    /// <summary>
    /// Gets a value of a field.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public TValue? GetFieldValue<TValue>(string name) => throw new InvalidOperationException(Error);

    /// <summary>
    /// Sets a value of a field.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public ILAccessor<T> SetFieldValue<TValue>(string name, TValue? value) => throw new InvalidOperationException(Error);
}
