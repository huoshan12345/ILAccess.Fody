using System;
using System.Runtime.CompilerServices;
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
#pragma warning disable IDE0051
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter

namespace ILAccess.Example;

public class Class
{
    private static void StaticPrivateMethod() { }
    private static int StaticPrivateField;
    private Class(int i) { PrivateField = i; }
    private void PrivateMethod() { }
    private int PrivateField;
    private int PrivateProperty { get => PrivateField; }
}

// Generic example
public class Class<T>
{
    private T? _field;
    private void M(T t) { }
    private void GM<U>(U u) { }
    private void GMWithConstraints<U, V>(U u, V v) where U : V, IEquatable<U> { }
}

internal class Accessors<V>
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_field")]
    public static extern ref V GetSetPrivateField(Class<V> c);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "M")]
    public static extern void CallM(Class<V> c, V v);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GM")]
    public static extern void CallGM<X>(Class<V> c, X x);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GMWithConstraints")]
    public static extern void CallGMWithConstraints<X, Y>(Class<V> c, X x, Y y) where X : Y, IEquatable<X>;
}

internal class Program
{
    public static void CallStaticPrivateMethod()
    {
        StaticPrivateMethod(null);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = nameof(StaticPrivateMethod))]
        static extern void StaticPrivateMethod(Class? c);
    }
    public static void GetSetStaticPrivateField()
    {
        ref var f = ref GetSetStaticPrivateField(null);

        [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "StaticPrivateField")]
        static extern ref int GetSetStaticPrivateField(Class? c);
    }
    public static void CallPrivateConstructor()
    {
        var c1 = PrivateCtor(1);
        var c2 = (Class)RuntimeHelpers.GetUninitializedObject(typeof(Class));
        PrivateCtorAsMethod(c2, 2);

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        static extern Class PrivateCtor(int i);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = ".ctor")]
        static extern void PrivateCtorAsMethod(Class c, int i);

    }
    public static void CallPrivateMethod(Class c)
    {
        PrivateMethod(c);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(PrivateMethod))]
        static extern void PrivateMethod(Class c);
    }
    public static void GetPrivateProperty(Class c)
    {
        var f = GetPrivateProperty(c);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_PrivateProperty")]
        static extern int GetPrivateProperty(Class c);
    }
    public static void GetSetPrivateField(Class c)
    {
        ref var f = ref GetSetPrivateField(c);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "PrivateField")]
        static extern ref int GetSetPrivateField(Class c);
    }

    public static void AccessGenericType(Class<int> c)
    {
        ref var f = ref Accessors<int>.GetSetPrivateField(c);
        Accessors<int>.CallM(c, 1);
        Accessors<int>.CallGM<string>(c, string.Empty);
        Accessors<int>.CallGMWithConstraints<string, object>(c, string.Empty, new object());
    }

    private static void Main(string[] args)
    {
        CallStaticPrivateMethod();
        GetSetStaticPrivateField();
        CallPrivateConstructor();

        var c = Activator.CreateInstance<Class>();
        CallPrivateMethod(c);
        GetPrivateProperty(c);
        GetSetPrivateField(c);
        AccessGenericType(new Class<int>());

        Console.Read();
    }
}
