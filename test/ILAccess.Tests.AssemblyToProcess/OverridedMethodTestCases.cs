using Xunit;

namespace ILAccess.Tests.AssemblyToProcess;

public class OverridedMethodTestCases
{
    [Fact]
    public void Property_Invoke()
    {
        var obj = new HasOverridedMethod();
        var result = obj.Base<IHasOverridedMethod>().Property;
        Assert.Equal("Property", result);
    }
    [Fact]
    public void OverridedMethod_Invoke()
    {
        var obj = new HasOverridedMethod();
        var result = obj.Base<IHasOverridedMethod>().Method(1, "a");
        Assert.Equal("Method(1, a)", result);
    }
    [Fact]
    public void OverridedMethod_InvokeTwice()
    {
        var obj = new HasOverridedMethod();
        var a = obj.Base<IHasOverridedMethod>().Method(1, "a");
        var b = obj.Base<IHasOverridedMethod>().Method(2, "b");
        var result = a + "----" + b;
        Assert.Equal("Method(1, a)----Method(2, b)", result);
    }
}
