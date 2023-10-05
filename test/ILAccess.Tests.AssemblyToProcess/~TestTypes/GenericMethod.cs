using System;

namespace ILAccess.Tests.AssemblyToProcess;

public interface IHasDefaultGenericMethod
{
    string Method(int x, string y) => $"{nameof(Method)}({x}, {y})";
    string Method<T>(int x, string y) => $"{nameof(Method)}<{typeof(T).Name}>({x}, {y})";
}

public class HasDefaultGenericMethod : IHasDefaultGenericMethod
{
    public string Method(int x, string y) => throw new InvalidOperationException();
    public string Method<T>(int x, string y) => throw new InvalidOperationException();
}

public interface IHasEmptyGenericMethod
{
    string Method(int x, string y);
    string Method<T>(int x, string y);
}

public interface IHasOverridedGenericMethod : IHasEmptyGenericMethod
{
    string IHasEmptyGenericMethod.Method(int x, string y) => $"{nameof(Method)}({x}, {y})";
    string IHasEmptyGenericMethod.Method<T>(int x, string y) => $"{nameof(Method)}<{typeof(T).Name}>({x}, {y})";
}

public class HasOverridedGenericMethod : IHasOverridedGenericMethod
{
    public string Method(int x, string y) => throw new InvalidOperationException();
    public string Method<T>(int x, string y) => throw new InvalidOperationException();
    public string Method<T>(T x, string y) => throw new InvalidOperationException();
}
