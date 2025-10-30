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

public struct TestStruct
{
    internal static int _staticValue = 42;
    internal int _value;
    internal TestStruct(int value) => _value = value;
    internal readonly string GetMessage(int code) => $"Current value: {_value}, code: {code}";
    internal static string GetStaticMessage(int code) => $"Current static value: {_staticValue}, code: {code}";
    internal readonly string GetString<T>(T item) => item?.ToString() ?? "";
    internal void SetValue(int value) => _value = value;
}

public static class TestStructAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_value")]
    public static extern ref int Value(this ref TestStruct instance);

    [ILAccessor(ILAccessorKind.StaticField, Name = "_staticValue")]
    public static extern ref int StaticValue(TestStruct instance);

    [ILAccessor(ILAccessorKind.Method, Name = "SetValue")]
    public static extern void SetValue(this ref TestStruct instance, int value);

    [ILAccessor(ILAccessorKind.Method, Name = "GetMessage")]
    public static extern string GetMessage(this TestStruct instance, int code);

    // NOTE: Unlike a reference type, the first argument cannot be TestStruct? here,
    // because Nullable<TestStruct> is different type from T TestStruct.
    [ILAccessor(ILAccessorKind.StaticMethod, Name = "GetStaticMessage")]
    public static extern string GetStaticMessage(TestStruct instance, int code);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestStruct Ctor(int x);

    [ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    public static extern string GetString<T>(this TestStruct c, T item);

    public static void Test()
    {
        {
            var model = Ctor(100);
            ref var value = ref model.Value();
            Console.WriteLine($"_value: {value}");

            value += 50;
            Console.WriteLine($"_value updated: {model._value}");

            model.SetValue(999);
            Console.WriteLine($"_value updated by method: {model._value}");

            ref var staticValue = ref StaticValue(model);
            Console.WriteLine($"_staticValue: {staticValue}");
            staticValue += 10;
            Console.WriteLine($"_staticValue updated: {staticValue}");

            var message = model.GetMessage(7);
            Console.WriteLine($"GetMessage: {message}");

            var staticMessage = GetStaticMessage(default, 7);
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
}