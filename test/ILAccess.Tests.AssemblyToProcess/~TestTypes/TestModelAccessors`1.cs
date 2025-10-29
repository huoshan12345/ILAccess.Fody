namespace ILAccess.Tests.AssemblyToProcess;

public static class GenericTestModelAccessors<T>
{
    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern T PrivateStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern T PublicStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern T PrivateField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern T PublicField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PrivateStaticField")]
    public static extern ref T RefPrivateStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.StaticField, Name = "PublicStaticField")]
    public static extern ref T RefPublicStaticField(TestModel<T>? c);

    [ILAccessor(ILAccessorKind.Field, Name = "PrivateField")]
    public static extern ref T RefPrivateField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.Field, Name = "PublicField")]
    public static extern ref T RefPublicField(TestModel<T> c);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern TestModel<T> Ctor(int i, string s, ref double rf);

    [ILAccessor(ILAccessorKind.Method, Name = ".ctor")]
    public static extern void CtorAsMethod(TestModel<T> c, int i, string s, ref double rf);

    [ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    public static extern string GetString(TestModel c, T item);

    //[ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    //public static extern string GetString<T1>(TestModel c, T1? item) where T1 : struct;

    //[ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    //public static extern string GetString<T1, T2>(TestModel c, T1 item, T2 item2);

    //[ILAccessor(ILAccessorKind.Method, Name = "GetString")]
    //public static extern string GetString<T2>(TestModel c, T item, T2 item2);
}

