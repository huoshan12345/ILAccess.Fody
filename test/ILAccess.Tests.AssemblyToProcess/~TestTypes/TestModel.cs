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
using System.Runtime.CompilerServices;
using System.Security.Claims;

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
    internal static int _start = 1;

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

    private static readonly Random _random = new(0);
    protected internal readonly int _i = _random.Next(100, 1000);
    protected internal readonly string _s = _random.NextString(10);
    protected internal readonly double _d = _random.NextDouble();

    internal TestModel() { }

    internal TestModel(int i, string s, ref double rd)
    {
        _i = i;
        _s = s;
        _d = rd;
    }

    private (int, double) Plus(int i, double d)
    {
        return (_i + i, _d + d);
    }

    private (int, string) Plus(int i, string s)
    {
        return (_i + i, _s + s);
    }

    private static int GetStart()
    {
        return _start;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private string GetString<T>(T item)
    {
        return item?.ToString() ?? "";
    }

    private string GetString<T>(T? item) where T : struct
    {
        return GetString(item.GetValueOrDefault());
    }

    private string GetString<T, T2>(T item, T2 item2)
    {
        return GetString(item) + GetString(item2);
    }

    private static string StaticGetString<T>(T item)
    {
        return item?.ToString() ?? "";
    }

    private static string StaticGetString<T>(T? item) where T : struct
    {
        return StaticGetString(item.GetValueOrDefault());
    }

    private static string StaticGetString<T, T2>(T item, T2 item2)
    {
        return StaticGetString(item) + StaticGetString(item2);
    }
}