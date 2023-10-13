namespace ILAccess;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ILAccessor<T>
{
    // NOTE: this type has to be declared as a class instead of struct,
    // because the IL instructions generated with struct are not easy to process as ones with class.

    internal ILAccessor(T? _) { }

    private const string Error = "This method is meant to be replaced at compile time by ILAccess.Fody, but the weaver has not been executed correctly.";

    /// <summary>
    /// Gets a value of a property.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public TValue? GetPropertyValue<TValue>(string name) => throw CreateException();

    /// <summary>
    /// Sets a value of a property.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public ILAccessor<T> SetPropertyValue<TValue>(string name, TValue? value) => throw CreateException();

    /// <summary>
    /// Gets a value of a field.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public TValue? GetFieldValue<TValue>(string name) => throw CreateException();

    /// <summary>
    /// Sets a value of a field.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public ILAccessor<T> SetFieldValue<TValue>(string name, TValue? value) => throw CreateException();

    internal static Exception CreateException() => throw new InvalidOperationException(Error);
}
