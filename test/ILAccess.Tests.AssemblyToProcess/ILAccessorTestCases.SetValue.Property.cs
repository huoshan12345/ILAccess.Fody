namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void SetValue_PublicStaticProperty()
    {
        using var _ = Disposable.Create(() => TestModel.PublicStaticProperty = 1);

        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PublicStaticProperty", 2);
        Assert.Equal(2, TestModel.PublicStaticProperty);
    }

    [Fact]
    public void SetValue_PublicProperty()
    {
        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PublicProperty", 2);
        Assert.Equal(2, obj.PublicProperty);
    }

    [Fact]
    public void SetValue_PrivateStaticProperty()
    {
        var prop = typeof(TestModel).GetRequiredProperty("PrivateStaticProperty");
        using var _ = Disposable.Create(() => prop.SetValue(null, 1));

        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PrivateStaticProperty", 2);
        Assert.Equal(2, prop.GetValue(null));
    }

    [Fact]
    public void SetValue_PrivateProperty()
    {
        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PrivateProperty", 2);
        Assert.Equal(2, typeof(TestModel).GetRequiredProperty("PrivateProperty").GetValue(obj));
    }
}
