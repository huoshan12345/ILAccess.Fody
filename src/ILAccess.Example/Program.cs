using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
// ReSharper disable ConvertToConstant.Local
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

public class TestModel
{
    private static int _staticValue = 42;
    private int _value;
    private TestModel(int value) => _value = value;
    private string GetMessage(int code) 
        => $"Current value: {_value}, code: {code}";
    private static string GetStaticMessage(int code) 
        => $"Current static value: {_staticValue}, code: {code}";
}

public static class Accessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static extern ref int Value(TestModel instance);

    [ILAccessor(ILAccessorKind.Field, Name = "_staticValue")]
    public static extern ref int StaticValue(TestModel instance);

    [ILAccessor(ILAccessorKind.Method, Name = "GetMessage")]
    public static extern string GetMessage(TestModel instance, int code);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "GetStaticMessage")]
    public static extern string GetStaticMessage(TestModel? instance, int code);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel NewTestModel(int x);
}

internal class Program
{
    private static void Main(string[] args)
    {
        var model = Accessors.NewTestModel(100);
        ref var value = ref Accessors.Value(model);
        Console.WriteLine($"_value: {value}");

        value += 50;
        Console.WriteLine($"_value updated: {value}");

        ref var staticValue = ref Accessors.StaticValue(model);
        Console.WriteLine($"_staticValue: {staticValue}");
        staticValue += 10;
        Console.WriteLine($"_staticValue updated: {staticValue}");

        var message = Accessors.GetMessage(model, 7);
        Console.WriteLine($"GetMessage: {message}");

        var staticMessage = Accessors.GetStaticMessage(null, 7);
        Console.WriteLine($"GetStaticMessage: {message}");

        Console.Read();
    }
}

public static class ExceptionAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_message")]
    public static extern ref string Message(this Exception obj);

    [ILAccessor(ILAccessorKind.Field, Name = "_stackTraceString")]
    public static extern ref string StackTraceString(this Exception obj);

    [ILAccessor(ILAccessorKind.Method, Name = "GetStackTrace")]
    public static extern string GetStackTrace(this Exception obj);

    [ILAccessor(ILAccessorKind.Method, Name = nameof(Exception.GetBaseException))]
    public static extern string GetBaseException(this Exception obj);
    
    public static void Test()
    {
        var ex = new Exception("xxxxxx");

        ref var value = ref ex.Message();
        Console.WriteLine($"_message: {value}");

        ref var stackTraceString = ref ex.StackTraceString();
        stackTraceString = new StackTrace().ToString();
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        Console.WriteLine($"GetStackTrace: {ex.GetStackTrace()}");
        Console.WriteLine($"GetBaseException: {ex.GetBaseException()}");
    }
}