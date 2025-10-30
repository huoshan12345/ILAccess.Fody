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
// ReSharper disable once MemberCanBeMadeStatic.Local
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
    private string GetMessage(int code) => $"Current value: {_value}, code: {code}";
    private static string GetStaticMessage(int code) => $"Current static value: {_staticValue}, code: {code}";
    internal string GetString<T>(T item) => item?.ToString() ?? "";
}

public class TestModel<T>
{
    private static readonly Random _random = new(0);
    protected internal readonly int _i = _random.Next(100, 1000);
    protected internal readonly string _s = "xxxxxxxxxxx";
    protected internal readonly double _d = _random.NextDouble();
    private readonly T? _value;

    internal TestModel() { }

    internal TestModel(int i, string s, ref double rd)
    {
        _i = i;
        _s = s;
        _d = rd;
    }
}

public static class TestModelAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static extern ref int Value(this TestModel instance);

    [ILAccessor(ILAccessorKind.StaticField, Name = "_staticValue")]
    public static extern ref int StaticValue(TestModel instance);

    [ILAccessor(ILAccessorKind.Method, Name = "GetMessage")]
    public static extern string GetMessage(this TestModel instance, int code);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "GetStaticMessage")]
    public static extern string GetStaticMessage(TestModel? instance, int code);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel Ctor(int x);

    [ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    public static extern string GetString<T>(this TestModel c, T item);

    public static void Test()
    {
        {
            var model = Ctor(100);
            ref var value = ref model.Value();
            Console.WriteLine($"_value: {value}");

            value += 50;
            Console.WriteLine($"_value updated: {value}");

            ref var staticValue = ref StaticValue(model);
            Console.WriteLine($"_staticValue: {staticValue}");
            staticValue += 10;
            Console.WriteLine($"_staticValue updated: {staticValue}");

            var message = model.GetMessage(7);
            Console.WriteLine($"GetMessage: {message}");

            var staticMessage = GetStaticMessage(null, 7);
            Console.WriteLine($"GetStaticMessage: {staticMessage}");

            {
                var str = model.GetString(12345);
                Console.WriteLine($"GetString<int>: {str}");
            }
            {
                var str = model.GetString("xyz");
                Console.WriteLine($"GetString<string>: {str}");
            }
        }
    }

    public static void TestGeneric()
    {
        {
            var model = TestModelAccessors<string>.Ctor();
            Console.WriteLine($"_i: {model._i}");

            ref var value = ref TestModelAccessors<string>.Value(model);
            Console.WriteLine($"_value: {value}");

            value = "Hello, ILAccess!";
            Console.WriteLine($"_value updated: {value}");
        }
        {
            var model = TestModelAccessors<int>.Ctor();
            Console.WriteLine($"_i: {model._i}");

            ref var value = ref TestModelAccessors<int>.Value(model);
            Console.WriteLine($"_value: {value}");

            value = 12345;
            Console.WriteLine($"_value updated: {value}");
        }
    }
}

public static class TestModelAccessors<T>
{
    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor(int i, string s, ref double rf);

    [ILAccessor(ILAccessorKind.Method, Name = ".ctor")]
    public static extern void CtorAsMethod(TestModel<T> c, int i, string s, ref double rf);

    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static extern ref T Value(TestModel<T> instance);
}
