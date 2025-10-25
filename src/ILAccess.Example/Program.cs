using System;
using System.Reflection;
using System.Runtime.CompilerServices;
// ReSharper disable UnassignedField.Global

// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedVariable
#pragma warning disable CS0169 // Field is never used
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CS0414 // Field is assigned but its value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace ILAccess.Example;

internal class Program
{
    private static void Main(string[] args)
    {
        var ex = new Exception("xxxxxx");
        ref var value = ref ex.Message();
        Console.WriteLine($"Original value: {value}");

        Console.Read();
    }
}

public static class Extensions
{
    [ILAccessor(ILAccessorKind.Field, Name = "_message")]
    public static extern ref string Message(this Exception obj);
}