namespace ILAccess.Tests.AssemblyToProcess;

// for TestModel
public static partial class TestModelAccessors
{
    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel Ctor();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel Ctor(int i, string s, ref double rf);

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

    [ILAccessor(ILAccessorKind.Method, Name = ".ctor")]
    public static extern void CtorAsMethod(this TestModel c, int i, string s, ref double rd);

    [ILAccessor(ILAccessorKind.Method, Name = "Plus")]
    public static extern (int, string) Plus(this TestModel c, int i, string s);

    [ILAccessor(ILAccessorKind.Method, Name = "Plus")]
    public static extern (int, double) Plus(this TestModel c, int i, double d);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "GetStart")]
    public static extern int GetStart(TestModel? c);

    [ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    public static extern string GetString<T>(this TestModel c, T item);

    [ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    public static extern string GetString<T>(this TestModel c, T? item) where T : struct;

    [ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    public static extern string GetString<T, T2>(this TestModel c, T item, T2 item2);
}

// for TestModel<T>
partial class TestModelAccessors
{
    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor<T>();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor<T>(int i, string s, ref double rf);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern T PrivateStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern T PublicStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern T PrivateField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern T PublicField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref T RefPrivateStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref T RefPublicStaticField<T>(this TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref T RefPrivateField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref T RefPublicField<T>(this TestModel<T> c);

    [ILAccessor(ILAccessorKind.Method, Name = ".ctor")]
    public static extern void CtorAsMethod<T>(this TestModel<T> c, int i, string s, ref double rd);

    public static T PrivateStaticField_GenericAccessors<T>(this TestModel<T>? c) => GenericTestModelAccessors<T>.PrivateStaticField(c);
    public static T PublicStaticField_GenericAccessors<T>(this TestModel<T>? c) => GenericTestModelAccessors<T>.PublicStaticField(c);
    public static T PrivateField_GenericAccessors<T>(this TestModel<T> c) => GenericTestModelAccessors<T>.PrivateField(c);
    public static T PublicField_GenericAccessors<T>(this TestModel<T> c) => GenericTestModelAccessors<T>.PublicField(c);
    public static ref T RefPrivateStaticField_GenericAccessors<T>(this TestModel<T>? c) => ref GenericTestModelAccessors<T>.RefPrivateStaticField(c);
    public static ref T RefPublicStaticField_GenericAccessors<T>(this TestModel<T>? c) => ref GenericTestModelAccessors<T>.RefPublicStaticField(c);
    public static ref T RefPrivateField_GenericAccessors<T>(this TestModel<T> c) => ref GenericTestModelAccessors<T>.RefPrivateField(c);
    public static ref T RefPublicField_GenericAccessors<T>(this TestModel<T> c) => ref GenericTestModelAccessors<T>.RefPublicField(c);
}