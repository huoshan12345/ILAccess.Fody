using Xunit;

namespace ILAccess.Tests.AssemblyToProcess;

public class ReoverridedMethodTestCases
{
    [Fact]
    public void Property_InvokeTwice()
    {
        var obj = new HasReoverridedMethod();
        var result = obj.Base<IHasOverridedMethod>().Property + "----" + obj.Base<IHasReoverridedMethod>().Property;
        Assert.Equal("Property----IHasReoverridedMethod.Property", result);
    }

    [Fact]
    public void ReoverrideMethod_InvokeTwice()
    {
        var obj = new HasReoverridedMethod();
        var a = obj.Base<IHasOverridedMethod>().Method(1, "a");
        var b = obj.Base<IHasReoverridedMethod>().Method(2, "b");
        var result = a + "----" + b;
        Assert.Equal("Method(1, a)----IHasReoverridedMethod.Method(2, b)", result);
    }
}
