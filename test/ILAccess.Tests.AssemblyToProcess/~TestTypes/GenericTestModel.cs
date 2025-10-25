// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedVariable
// ReSharper disable UnassignedField.Global
#pragma warning disable CS0169 // Field is never used
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CS0414 // Field is assigned but its value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace ILAccess.Tests.AssemblyToProcess;

public class GenericTestModel<T> : TestModel
{
    private static T? PrivateStaticField;
    public new static T? PublicStaticField;
    private T? PrivateField;
    public new T? PublicField;
}

public static class GenericAccessors<T>
{
    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern T PrivateStaticField(GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern T PublicStaticField(GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern T PrivateField(GenericTestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern T PublicField(GenericTestModel<T> c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref T RefPrivateStaticField(GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref T RefPublicStaticField(GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref T RefPrivateField(GenericTestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref T RefPublicField(GenericTestModel<T> c);
}

public static partial class GenericTestModelExtensions
{
    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern T PrivateStaticField<T>(this GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern T PublicStaticField<T>(this GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern T PrivateField<T>(this GenericTestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern T PublicField<T>(this GenericTestModel<T> c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref T RefPrivateStaticField<T>(this GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref T RefPublicStaticField<T>(this GenericTestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref T RefPrivateField<T>(this GenericTestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref T RefPublicField<T>(this GenericTestModel<T> c);
}

public static partial class GenericTestModelExtensions
{
    public static T PrivateStaticField_GenericAccessors<T>(this GenericTestModel<T>? c) => GenericAccessors<T>.PrivateStaticField(c);
    public static T PublicStaticField_GenericAccessors<T>(this GenericTestModel<T>? c) => GenericAccessors<T>.PublicStaticField(c);
    public static T PrivateField_GenericAccessors<T>(this GenericTestModel<T> c) => GenericAccessors<T>.PrivateField(c);
    public static T PublicField_GenericAccessors<T>(this GenericTestModel<T> c) => GenericAccessors<T>.PublicField(c);
    public static ref T RefPrivateStaticField_GenericAccessors<T>(this GenericTestModel<T>? c) => ref GenericAccessors<T>.RefPrivateStaticField(c);
    public static ref T RefPublicStaticField_GenericAccessors<T>(this GenericTestModel<T>? c) => ref GenericAccessors<T>.RefPublicStaticField(c);
    public static ref T RefPrivateField_GenericAccessors<T>(this GenericTestModel<T> c) => ref GenericAccessors<T>.RefPrivateField(c);
    public static ref T RefPublicField_GenericAccessors<T>(this GenericTestModel<T> c) => ref GenericAccessors<T>.RefPublicField(c);
}