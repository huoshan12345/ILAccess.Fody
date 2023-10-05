using Xunit;

namespace ILAccess.Tests.AssemblyToProcess;

public class OverridedGenericMethodTestCases
{
    [Fact]
    public void Method_Invoke()
    {
        var obj = new HasOverridedGenericMethod();
        var result = obj.Base<IHasOverridedGenericMethod>().Method(1, "a");
        Assert.Equal("Method(1, a)", result);
    }
    [Fact]
    public void Method_InvokeTwice()
    {
        var obj = new HasOverridedGenericMethod();
        var a = obj.Base<IHasOverridedGenericMethod>().Method(1, "a");
        var b = obj.Base<IHasOverridedGenericMethod>().Method(2, "b");
        var result = a + "----" + b;
        Assert.Equal("Method(1, a)----Method(2, b)", result);
    }
    [Fact]
    public void GenericMethod_Invoke()
    {
        var obj = new HasOverridedGenericMethod();
        var result = obj.Base<IHasOverridedGenericMethod>().Method<int>(2, "b");
        Assert.Equal("Method<Int32>(2, b)", result);
    }
    [Fact]
    public void GenericMethod_InvokeTwice()
    {
        var obj = new HasOverridedGenericMethod();
        var a = obj.Base<IHasOverridedGenericMethod>().Method<int>(1, "a");
        var b = obj.Base<IHasOverridedGenericMethod>().Method<string>(2, "b");
        var result = a + "----" + b;
        Assert.Equal("Method<Int32>(1, a)----Method<String>(2, b)", result);
    }
}
