using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA2211
#pragma warning disable IDE0051
#pragma warning disable IDE0044
#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace ILAccess.Tests.AssemblyToProcess;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
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

public static class TestModelAccessors
{
    [ILAccess(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref int PrivateStaticField(this TestModel? c);

    [ILAccess(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref int PublicStaticField(this TestModel? c);

    [ILAccess(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref int PrivateField(this TestModel c);

    [ILAccess(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref int PublicField(this TestModel c);
}
