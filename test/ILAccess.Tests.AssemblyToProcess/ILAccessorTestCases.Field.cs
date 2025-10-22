namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void PublicStaticField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.PublicStaticField();

        var original = TestModel.PublicStaticField;
        using var _ = Disposable.Create(() => TestModel.PublicStaticField = original); // Restore original value after test

        Assert.Equal(original, value);

        const int newValue = 1122334455;
        value = newValue;
        Assert.Equal(newValue, TestModel.PublicStaticField);
    }

    [Fact]
    public void PrivateStaticField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.PrivateStaticField();

        var original = Get();
        using var _ = Disposable.Create(() => Set(original)); // Restore original value after test

        Assert.Equal(original, value);

        const int newValue = 1122334455;
        value = newValue;
        Assert.Equal(newValue, Get());

        static int Get()
        {
            return (int)typeof(TestModel).GetRequiredField("PrivateStaticField").GetValue(null)!;
        }

        static void Set(int value)
        {
            typeof(TestModel).GetRequiredField("PrivateStaticField").SetValue(null, value);
        }
    }

    [Fact]
    public void PublicField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.PublicField();
        Assert.Equal(obj.PublicField, value);

        const int newValue = 1122334455;
        value = newValue;
        Assert.Equal(newValue, obj.PublicField);
    }

    [Fact]
    public void PrivateField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.PrivateField();
        Assert.Equal(Get(obj), value);

        const int newValue = 1122334455;
        value = newValue;
        Assert.Equal(newValue, Get(obj));

        static int Get(TestModel obj)
        {
            return (int)typeof(TestModel).GetRequiredField("PrivateField").GetValue(obj)!;
        }
    }
}
