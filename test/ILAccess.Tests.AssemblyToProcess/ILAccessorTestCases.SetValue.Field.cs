namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void SetValue_PublicStaticField()
    {
        using var _ = Disposable.Create(() => TestModel.PublicStaticField = 1);

        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PublicStaticField", 2);
        Assert.Equal(2, TestModel.PublicStaticField);
    }

    [Fact]
    public void SetValue_PublicField()
    {
        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PublicField", 2);
        Assert.Equal(2, obj.PublicField);
    }

    [Fact]
    public void SetValue_PrivateStaticField()
    {
        var prop = typeof(TestModel).GetRequiredField("PrivateStaticField");
        using var _ = Disposable.Create(() => prop.SetValue(null, 1));

        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PrivateStaticField", 2);
        Assert.Equal(2, prop.GetValue(null));
    }

    [Fact]
    public void SetValue_PrivateField()
    {
        var obj = new TestModel();
        obj.ILAccess().SetValue<int>("PrivateField", 2);
        Assert.Equal(2, typeof(TestModel).GetRequiredField("PrivateField").GetValue(obj));
    }
}
