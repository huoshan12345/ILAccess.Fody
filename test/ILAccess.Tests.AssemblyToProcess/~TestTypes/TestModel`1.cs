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

public class TestModel<T> : TestModel
{
    internal TestModel() { }

    internal TestModel(int i, string s, ref double rd)
        : base(i, s, ref rd) { }

    private static T? PrivateStaticField;
    public new static T? PublicStaticField;
    private T? PrivateField;
    public new T? PublicField;
}

partial class Accessors
{
    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor<T>();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor<T>(int i, string s, ref double rf);
}

public static class GenericAccessors<T>
{
    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern T PrivateStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern T PublicStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern T PrivateField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern T PublicField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref T RefPrivateStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref T RefPublicStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref T RefPrivateField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref T RefPublicField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor(int i, string s, ref double rf);

    [ILAccessor(ILAccessorKind.Method, Name = ".ctor")]
    public static extern void PrivateCtorAsMethod(TestModel<T> c, int i, string s, ref double rf);
}

public static partial class GenericTestModelExtensions
{
    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern T PrivateStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern T PublicStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern T PrivateField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern T PublicField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref T RefPrivateStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref T RefPublicStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref T RefPrivateField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref T RefPublicField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.Method, Name = ".ctor")]
    public static extern void PrivateCtorAsMethod<T>(this TestModel<T> c, int i, string s, ref double rf);
}

public static partial class GenericTestModelExtensions
{
    public static T PrivateStaticField_GenericAccessors<T>(this TestModel<T>? c) => GenericAccessors<T>.PrivateStaticField(c);
    public static T PublicStaticField_GenericAccessors<T>(this TestModel<T>? c) => GenericAccessors<T>.PublicStaticField(c);
    public static T PrivateField_GenericAccessors<T>(this TestModel<T> c) => GenericAccessors<T>.PrivateField(c);
    public static T PublicField_GenericAccessors<T>(this TestModel<T> c) => GenericAccessors<T>.PublicField(c);
    public static ref T RefPrivateStaticField_GenericAccessors<T>(this TestModel<T>? c) => ref GenericAccessors<T>.RefPrivateStaticField(c);
    public static ref T RefPublicStaticField_GenericAccessors<T>(this TestModel<T>? c) => ref GenericAccessors<T>.RefPublicStaticField(c);
    public static ref T RefPrivateField_GenericAccessors<T>(this TestModel<T> c) => ref GenericAccessors<T>.RefPrivateField(c);
    public static ref T RefPublicField_GenericAccessors<T>(this TestModel<T> c) => ref GenericAccessors<T>.RefPublicField(c);
}