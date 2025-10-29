using System;
using System.Collections.Generic;
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

public static class TestModelAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static extern ref int Value(TestModel instance);

    [ILAccessor(ILAccessorKind.StaticField, Name = "_staticValue")]
    public static extern ref int StaticValue(TestModel instance);

    [ILAccessor(ILAccessorKind.Method, Name = "GetMessage")]
    public static extern string GetMessage(TestModel instance, int code);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "GetStaticMessage")]
    public static extern string GetStaticMessage(TestModel? instance, int code);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel Ctor(int x);
}

public class TestModel<T>
{
    private static readonly Random _random = new(0);
    protected internal readonly int _i = _random.Next(100, 1000);
    protected internal readonly string _s = "xxxxxxxxxxx";
    protected internal readonly double _d = _random.NextDouble();

    internal TestModel() { }

    internal TestModel(int i, string s, ref double rd)
    {
        _i = i;
        _s = s;
        _d = rd;
    }
}

public static class TestModelAccessors<T>
{
    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor(int i, string s, ref double rf);
}

public static class TestModelExtensions
{
    [ILAccessor(ILAccessorKind.Method, Name = ".ctor")]
    public static extern void CtorAsMethod<T>(this TestModel<T> c, int i, string s, ref double rf);
}

internal class Program
{
    private static void Main(string[] args)
    {
        {
            var model = TestModelAccessors.Ctor(100);
            ref var value = ref TestModelAccessors.Value(model);
            Console.WriteLine($"_value: {value}");

            value += 50;
            Console.WriteLine($"_value updated: {value}");

            ref var staticValue = ref TestModelAccessors.StaticValue(model);
            Console.WriteLine($"_staticValue: {staticValue}");
            staticValue += 10;
            Console.WriteLine($"_staticValue updated: {staticValue}");

            var message = TestModelAccessors.GetMessage(model, 7);
            Console.WriteLine($"GetMessage: {message}");

            var staticMessage = TestModelAccessors.GetStaticMessage(null, 7);
            Console.WriteLine($"GetStaticMessage: {message}");
        }

        {
            var model = TestModelAccessors<string>.Ctor();
            Console.WriteLine($"_i: {model._i}");
        }


        Console.WriteLine();
        Console.WriteLine("Testing Exception accessors");
        ExceptionAccessors.Test();

        Console.WriteLine();
        Console.WriteLine("Testing List<T> accessors");
        ListAccessors.Test();

        Console.WriteLine();
        Console.WriteLine("Testing List<T> accessors<T>");
        ListAccessors<int>.Test();

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

public static class ListAccessors<T>
{
    [ILAccessor(ILAccessorKind.Field, Name = "_items")]
    public static extern ref T[] Items(List<T> obj);

    [ILAccessor(ILAccessorKind.Method, Name = "Grow")]
    public static extern void Grow(List<T> obj, int capacity);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor();

    public static void Test()
    {
        var list = ListAccessors<string>.Ctor();
        list.Add("xxxxxxxxxxx");
        Console.WriteLine($"List[0]: {list[0]}");

        ref var items = ref ListAccessors<string>.Items(list);
        items[0] = "yyyyyy";
        Console.WriteLine($"List[0] after set items: {list[0]}");

        Console.WriteLine($"Capacity: {list.Capacity}");
        ListAccessors<string>.Grow(list, 100);
        Console.WriteLine($"Capacity after Grow: {list.Capacity}");
    }
}

public static class ListAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_items")]
    public static extern ref T[] Items<T>(this List<T> obj);

    [ILAccessor(ILAccessorKind.Method, Name = "Grow")]
    public static extern void Grow<T>(this List<T> obj, int capacity);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor<T>();

    public static void Add<T>(this List<T> obj, T item)
    {
        obj.Add(item);
    }

    public static void Test()
    {
        var list = Ctor<string>();
        list.Add("xxxxxxxxxxx");
        Console.WriteLine($"List[0]: {list[0]}");

        ref var items = ref list.Items();
        items[0] = "yyyyyy";
        Console.WriteLine($"List[0] after set items: {list[0]}");

        Console.WriteLine($"Capacity: {list.Capacity}");
        list.Grow(100);
        Console.WriteLine($"Capacity after Grow: {list.Capacity}");
    }
}