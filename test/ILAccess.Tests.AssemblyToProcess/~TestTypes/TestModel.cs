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

public class TestModel
{
    private static int _start = 1;

    private static int PrivateStaticField = _start++;
    public static int PublicStaticField = _start++;
    private int PrivateField = _start++;
    public int PublicField = _start++;

    private static readonly int PrivateStaticReadonlyField = _start++;
    public static readonly int PublicStaticReadonlyField = _start++;
    private readonly int PrivateReadonlyField = _start++;
    public readonly int PublicReadonlyField = _start++;

    private static int PrivateStaticProperty { get; set; } = _start++;
    public static int PublicStaticProperty { get; set; } = _start++;
    private int PrivateProperty { get; set; } = _start++;
    public int PublicProperty { get; set; } = _start++;

    public int PublicPropertyWithPrivateSetter { get; private set; } = _start++;
    public int PublicPropertyWithPrivateGetter { private get; set; } = _start++;
    public int PublicPropertyWithoutSetter { get; } = _start++;
}

public class GenericTestModel<T> : TestModel
{
    private static T? PrivateStaticField;
    public new static T? PublicStaticField;
    private T? PrivateField;
    public new T? PublicField;
}

public static class TestModelAccessors
{
    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern int PrivateStaticField(this TestModel? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern int PublicStaticField(this TestModel? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern int PrivateField(this TestModel c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern int PublicField(this TestModel c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref int RefPrivateStaticField(this TestModel? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref int RefPublicStaticField(this TestModel? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref int RefPrivateField(this TestModel c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref int RefPublicField(this TestModel c);
}

public static class TestModelAccessors<T>
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

    public static T? _PublicStaticField(GenericTestModel<T>? c) => GenericTestModel<T>.PublicStaticField;
}

public static class TestModelExtensions
{
    public static T PrivateStaticField<T>(this GenericTestModel<T>? c) => TestModelAccessors<T>.PrivateStaticField(c);
    public static T PublicStaticField<T>(this GenericTestModel<T>? c) => TestModelAccessors<T>.PublicStaticField(c);
    public static T PrivateField<T>(this GenericTestModel<T> c) => TestModelAccessors<T>.PrivateField(c);
    public static T PublicField<T>(this GenericTestModel<T> c) => TestModelAccessors<T>.PublicField(c);
    public static ref T RefPrivateStaticField<T>(this GenericTestModel<T>? c) => ref TestModelAccessors<T>.RefPrivateStaticField(c);
    public static ref T RefPublicStaticField<T>(this GenericTestModel<T>? c) => ref TestModelAccessors<T>.RefPublicStaticField(c);
    public static ref T RefPrivateField<T>(this GenericTestModel<T> c) => ref TestModelAccessors<T>.RefPrivateField(c);
    public static ref T RefPublicField<T>(this GenericTestModel<T> c) => ref TestModelAccessors<T>.RefPublicField(c);

    public static T? _PublicStaticField<T>(this GenericTestModel<T>? c) => GenericTestModel<T>.PublicStaticField;
}