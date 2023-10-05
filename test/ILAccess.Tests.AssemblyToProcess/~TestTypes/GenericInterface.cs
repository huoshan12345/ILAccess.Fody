using System;

namespace ILAccess.Tests.AssemblyToProcess;

public interface IGenericHasDefaultGenericMethod<in T>
{
    string Method(int x, string y) => $"{nameof(Method)}({x}, {y})";
    string Method(T x, string y) => $"{nameof(Method)}({typeof(T).Name} {x}, {y})";
    string Method<TParameter>(int x, string y) => $"{nameof(Method)}<{typeof(TParameter).Name}>({x}, {y})";
    string Method<TParameter>(int x, TParameter y) => $"{nameof(Method)}<{typeof(TParameter).Name}>({x}, {typeof(TParameter).Name} {y})";
    string Method<TParameter>(T x, TParameter y) => $"{nameof(Method)}<{typeof(TParameter).Name}>({typeof(T).Name} {x}, {typeof(TParameter).Name} {y})";
}

public class GenericInterface<T> : IGenericHasDefaultGenericMethod<T>
{
    string IGenericHasDefaultGenericMethod<T>.Method(int x, string y) => throw new InvalidOperationException();
    string IGenericHasDefaultGenericMethod<T>.Method(T x, string y) => throw new InvalidOperationException();
    string IGenericHasDefaultGenericMethod<T>.Method<TParameter>(int x, string y) => throw new InvalidOperationException();
    string IGenericHasDefaultGenericMethod<T>.Method<TParameter>(int x, TParameter y) => throw new InvalidOperationException();
    string IGenericHasDefaultGenericMethod<T>.Method<TParameter>(T x, TParameter y) => throw new InvalidOperationException();
}



public interface IGenericHasEmptyGenericMethod<in T>
{
    string Method(int x, string y);
    string Method(T x, string y);
    string Method<TParameter>(int x, string y);
    string Method<TParameter>(int x, TParameter y);
    string Method<TParameter>(T x, TParameter y);
}

public interface IGenericHasOverridedGenericMethod<in T> : IGenericHasEmptyGenericMethod<T>
{
    string IGenericHasEmptyGenericMethod<T>.Method(int x, string y) => $"{nameof(Method)}({x}, {y})";
    string IGenericHasEmptyGenericMethod<T>.Method(T x, string y) => $"{nameof(Method)}({typeof(T).Name} {x}, {y})";
    string IGenericHasEmptyGenericMethod<T>.Method<TParameter>(int x, string y) => $"{nameof(Method)}<{typeof(TParameter).Name}>({x}, {y})";
    string IGenericHasEmptyGenericMethod<T>.Method<TParameter>(int x, TParameter y) => $"{nameof(Method)}<{typeof(TParameter).Name}>({x}, {typeof(TParameter).Name} {y})";
    string IGenericHasEmptyGenericMethod<T>.Method<TParameter>(T x, TParameter y) => $"{nameof(Method)}<{typeof(TParameter).Name}>({typeof(T).Name} {x}, {typeof(TParameter).Name} {y})";
}

public class GenericHasOverridedGenericMethod<T> : IGenericHasOverridedGenericMethod<T>
{
    string IGenericHasEmptyGenericMethod<T>.Method(int x, string y) => throw new InvalidOperationException();
    string IGenericHasEmptyGenericMethod<T>.Method(T x, string y) => throw new InvalidOperationException();
    string IGenericHasEmptyGenericMethod<T>.Method<TParameter>(int x, string y) => throw new InvalidOperationException();
    string IGenericHasEmptyGenericMethod<T>.Method<TParameter>(int x, TParameter y) => throw new InvalidOperationException();
    string IGenericHasEmptyGenericMethod<T>.Method<TParameter>(T x, TParameter y) => throw new InvalidOperationException();
}
