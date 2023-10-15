namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void GetValue_PublicStaticProperty()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicStaticProperty");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateStaticProperty()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateStaticProperty");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PublicProperty()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicProperty");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateProperty()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateProperty");
        Assert.Equal(1, value);
    }
}
