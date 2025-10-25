// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedVariable
// ReSharper disable UnassignedField.Global

using System;

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

    internal TestModel() { }

    private static readonly Random _random = new(0);
    internal readonly int _i = _random.Next(100, 1000);
    internal readonly string _s = _random.NextString(10);
    internal readonly double _d = _random.NextDouble();

    internal TestModel(int i, string s, ref double rd)
    {
        _i = i;
        _s = s;
        _d = rd;
    }
}

public static class TestModelExtensions
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

public static class Accessors
{
    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel Ctor(TestModel? c = null);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel Ctor(TestModel? c, int i, string s, ref double rf);

    public static TestModel _Ctor(TestModel? c, int i, string s, ref double rf) => new(i, s, ref rf);
}