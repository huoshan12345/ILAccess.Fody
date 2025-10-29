using System;
using System.Collections.Generic;

namespace ILAccess.Tests.AssemblyToProcess;

public static class ExceptionAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_message")]
    public static extern ref string Message(this Exception obj);

    [ILAccessor(ILAccessorKind.Method, Name = "GetClassName")]
    public static extern string GetClassName(this Exception obj);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "IsImmutableAgileException")]
    public static extern bool IsImmutableAgileException(Exception? obj, Exception exception);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern Exception Ctor();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern Exception Ctor(string message);
}

public static class ListAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_items")]
    public static extern ref T[] Items<T>(this List<T> obj);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "IsCompatibleObject")]
    public static extern bool IsCompatibleObject<T>(this List<T> obj, object? value);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor<T>();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor<T>(int capacity);
}

public static class GenericListAccessors<T>
{
    [ILAccessor(ILAccessorKind.Field, Name = "_items")]
    public static extern ref T[] Items(List<T> obj);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "IsCompatibleObject")]
    public static extern bool IsCompatibleObject(List<T> obj, object? value);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor();

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor(int capacity);
}