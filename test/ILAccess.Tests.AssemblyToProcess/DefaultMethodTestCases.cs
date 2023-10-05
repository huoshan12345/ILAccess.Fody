using System;
using Xunit;

namespace ILAccess.Tests.AssemblyToProcess;

public class DefaultMethodTestCases
{
    [Fact]
    public void Property_Invoke()
    {
        var result = new HasDefaultMethod().Base<IHasDefaultMethod>().Property;
        Assert.Equal("Property", result);
    }
    [Fact]
    public void DefaultMethod_Invoke()
    {
        var obj = new HasDefaultMethod();
        var result = obj.Base<IHasDefaultMethod>().Method(1, "a");
        Assert.Equal("Method.1.a", result);
    }
    [Fact]
    public void DefaultMethod_ComplexArguments_Invoke()
    {
        var obj = new HasDefaultMethod();
        var result = obj.Base<IHasDefaultMethod>().Method(1 + (int)Math.Pow(2, 3), nameof(DefaultMethodTestCases) + "." + nameof(DefaultMethod_ComplexArguments_Invoke));
        Assert.Equal("Method.9.DefaultMethodTestCases.DefaultMethod_ComplexArguments_Invoke", result);
    }
    [Fact]
    public void DefaultMethod_InvokeTwice()
    {
        var obj = new HasDefaultMethod();
        var a = obj.Base<IHasDefaultMethod>().Method(1, "a");
        var b = obj.Base<IHasDefaultMethod>().Method(2, "b");
        var result = a + "----" + b;
        Assert.Equal("Method.1.a----Method.2.b", result);
    }
}
