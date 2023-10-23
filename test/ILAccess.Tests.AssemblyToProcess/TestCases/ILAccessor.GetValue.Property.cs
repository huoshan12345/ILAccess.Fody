namespace ILAccess.Tests.AssemblyToProcess.TestCases;

public partial class ILAccessor
{
    [Fact]
    public void GetValue_PublicStaticProperty()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicStaticProperty");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateStaticProperty()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateStaticProperty");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PublicProperty()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicProperty");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateProperty()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateProperty");
        Assert.Equal(1, value);
    }
}
