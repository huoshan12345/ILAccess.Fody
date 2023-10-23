namespace ILAccess.Tests.AssemblyToProcess.TestCases;

public partial class ILAccessor
{
    [Fact]
    public void GetValue_PublicStaticField()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicStaticField");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateStaticField()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateStaticField");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PublicField()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PublicField");
        Assert.Equal(1, value);
    }

    [Fact]
    public void GetValue_PrivateField()
    {
        object obj = new TestModel();
        var value = obj.ILAccess().GetValue<int>("PrivateField");
        Assert.Equal(1, value);
    }
}
