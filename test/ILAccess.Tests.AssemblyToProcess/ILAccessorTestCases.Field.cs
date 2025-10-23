namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void PublicStaticField_Get()
    {
        var obj = new TestModel();
        var value = obj.PublicStaticField();
        Assert.Equal(TestModel.PublicStaticField, value);
    }

    [Fact]
    public void PrivateStaticField_Get()
    {
        var obj = new TestModel();
        var value = obj.PrivateStaticField();
        Assert.Equal(Get(), value);

        static int Get()
        {
            return (int)typeof(TestModel).GetRequiredField("PrivateStaticField").GetValue(null)!;
        }
    }

    [Fact]
    public void PublicField_Get()
    {
        var obj = new TestModel();
        var value = obj.PublicField();
        Assert.Equal(obj.PublicField, value);
    }

    [Fact]
    public void PrivateField_Get()
    {
        var obj = new TestModel();
        var value = obj.PrivateField();
        Assert.Equal(Get(obj), value);

        static int Get(TestModel obj)
        {
            return (int)typeof(TestModel).GetRequiredField("PrivateField").GetValue(obj)!;
        }
    }
}
