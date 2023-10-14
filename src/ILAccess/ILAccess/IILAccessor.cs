namespace ILAccess;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IILAccessor<T>
{
    /// <summary>
    /// Gets a value of a property or field.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    TValue? GetValue<TValue>(string name);

    /// <summary>
    /// Sets a value of a property or field.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    void SetValue<TValue>(string name, TValue? value);
}
