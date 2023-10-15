namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void GetValue_PublicStaticField()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicStaticField");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateStaticField()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateStaticField");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PublicField()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicField");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateField()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateField");
        Assert.Equal(1, value);
    }
}
