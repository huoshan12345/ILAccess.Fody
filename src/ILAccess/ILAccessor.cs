namespace ILAccess;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly record struct ILAccessor<T>(T? Value)
{
    /// <summary>
    /// Gets a value of a property.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [ILAccess] public extern TValue? GetPropertyValue<TValue>(string name);

    /// <summary>
    /// Sets a value of a property.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [ILAccess] public extern ILAccessor<T> SetPropertyValue<TValue>(string name, TValue? value);

    /// <summary>
    /// Gets a value of a field.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [ILAccess] public extern TValue? GetFieldValue<TValue>(string name);

    /// <summary>
    /// Sets a value of a field.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [ILAccess] public extern ILAccessor<T> SetFieldValue<TValue>(string name, TValue? value);
}
