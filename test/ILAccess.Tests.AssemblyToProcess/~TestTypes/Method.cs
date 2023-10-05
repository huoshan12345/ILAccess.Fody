using System;

namespace ILAccess.Tests.AssemblyToProcess;

public interface IHasDefaultMethod
{
    string Property => $"{nameof(Property)}";
    string Method(int x, string y) => $"{nameof(Method)}.{x}.{y}";
}

public class HasDefaultMethod : IHasDefaultMethod
{
    public string Property => throw new InvalidOperationException();
    public virtual string Method(int x, string y) => throw new InvalidOperationException();
}

public class DerivedHasDefaultMethod : HasDefaultMethod
{
    public override string Method(int x, string y) => throw new InvalidOperationException();
}

public interface IHasEmptyMethod
{
    string Property { get; }
    string Method(int x, string y);
}

public interface IHasOverridedMethod : IHasEmptyMethod
{
    string IHasEmptyMethod.Property => $"{nameof(Property)}";
    string IHasEmptyMethod.Method(int x, string y) => $"{nameof(Method)}({x}, {y})";
}

public class HasOverridedMethod : IHasOverridedMethod
{
    public string Property => throw new InvalidOperationException();
    public string Method(int x, string y) => throw new InvalidOperationException();
}

public interface IHasReoverridedMethod : IHasOverridedMethod
{
    string IHasEmptyMethod.Property => $"{nameof(IHasReoverridedMethod)}.{nameof(Property)}";
    string IHasEmptyMethod.Method(int x, string y) => $"{nameof(IHasReoverridedMethod)}.{nameof(Method)}({x}, {y})";
}

public class HasReoverridedMethod : IHasReoverridedMethod
{
    public string Property => throw new InvalidOperationException();
    public string Method(int x, string y) => throw new InvalidOperationException();
}
