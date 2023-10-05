using Xunit;

namespace ILAccess.Tests.AssemblyToProcess;

public class GenericInterfaceTestCases
{
    [Fact]
    public void Method_Invoke()
    {
        var obj = new GenericInterface<int>();
        var result = obj.Base<IGenericHasDefaultGenericMethod<int>>().Method(1, "a");
        Assert.Equal("Method(1, a)", result);
    }
    [Fact]
    public void Method_InvokeTwice()
    {
        var obj = new GenericInterface<string>();
        var a = obj.Base<IGenericHasDefaultGenericMethod<string>>().Method(1, "a");
        var b = obj.Base<IGenericHasDefaultGenericMethod<string>>().Method(2, "b");
        var result = a + "----" + b;
        Assert.Equal("Method(1, a)----Method(2, b)", result);
    }
    [Fact]
    public void GenericMethod_Invoke()
    {
        var obj = new GenericInterface<int>();
        var result = obj.Base<IGenericHasDefaultGenericMethod<int>>().Method<string>(1, "a");
        Assert.Equal("Method<String>(1, a)", result);
    }
    [Fact]
    public void GenericMethod_InvokeTwice()
    {
        var obj = new GenericInterface<string>();
        var a = obj.Base<IGenericHasDefaultGenericMethod<string>>().Method<string>(1, "a");
        var b = obj.Base<IGenericHasDefaultGenericMethod<string>>().Method<int>(2, "b");
        var result = a + "----" + b;
        Assert.Equal("Method<String>(1, a)----Method<Int32>(2, b)", result);
    }
}
