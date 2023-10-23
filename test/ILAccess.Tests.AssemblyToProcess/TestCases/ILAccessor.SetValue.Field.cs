namespace ILAccess.Tests.AssemblyToProcess.TestCases;

public partial class ILAccessor
{
    [Fact]
    public void SetValue_PublicStaticField()
    {
        using var _ = Disposable.Create(() => TestModel.PublicStaticField = 1);

        object obj = new TestModel();
        obj.ILAccess().SetValue<int>("PublicStaticField", 2);
        Assert.Equal(2, TestModel.PublicStaticField);
    }

    [Fact]
    public void SetValue_PublicField()
    {
        object obj = new TestModel();
        obj.ILAccess().SetValue<int>("PublicField", 2);
        Assert.Equal(2, ((TestModel)obj).PublicField);
    }

    [Fact]
    public void SetValue_PrivateStaticField()
    {
        var prop = typeof(TestModel).GetRequiredField("PrivateStaticField");
        using var _ = Disposable.Create(() => prop.SetValue(null, 1));

        object obj = new TestModel();
        obj.ILAccess().SetValue<int>("PrivateStaticField", 2);
        Assert.Equal(2, prop.GetValue(null));
    }

    [Fact]
    public void SetValue_PrivateField()
    {
        object obj = new TestModel();
        obj.ILAccess().SetValue<int>("PrivateField", 2);
        Assert.Equal(2, typeof(TestModel).GetRequiredField("PrivateField").GetValue(obj));
    }
}
