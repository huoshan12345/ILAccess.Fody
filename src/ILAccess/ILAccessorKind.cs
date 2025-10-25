namespace ILAccess;

#pragma warning disable CS0436
/// <summary>
/// Specifies the kind of target to which an <see cref="ILAccessorAttribute" /> is providing access.
/// </summary>
public enum ILAccessorKind
{
    /// <summary>
    /// Provide access to a constructor.
    /// </summary>
    Constructor,

    /// <summary>
    /// Provide access to a method.
    /// </summary>
    Method,

    /// <summary>
    /// Provide access to a static method.
    /// </summary>
    StaticMethod,

    /// <summary>
    /// Provide access to a field.
    /// </summary>
    Field,

    /// <summary>
    /// Provide access to a static field.
    /// </summary>
    StaticField,
}