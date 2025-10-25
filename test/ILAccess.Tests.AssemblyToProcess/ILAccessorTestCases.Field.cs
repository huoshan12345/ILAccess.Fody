using System;
using System.Reflection;
// ReSharper disable ConvertToConstant.Local

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

    [Fact]
    public void RefPublicStaticField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.RefPublicStaticField();

        var original = TestModel.PublicStaticField;
        using var _ =
            Disposable.Create(() => TestModel.PublicStaticField = original); // Restore original value after test

        Assert.Equal(original, value);

        const int newValue = 1122334455;
        value = newValue;
        Assert.Equal(newValue, TestModel.PublicStaticField);
    }

    [Fact]
    public void RefPrivateStaticField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.RefPrivateStaticField();

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
    public void RefPublicField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.RefPublicField();
        Assert.Equal(obj.PublicField, value);

        const int newValue = 1122334455;
        value = newValue;
        Assert.Equal(newValue, obj.PublicField);
    }

    [Fact]
    public void RefPrivateField_Get_Set()
    {
        var obj = new TestModel();
        ref var value = ref obj.RefPrivateField();
        Assert.Equal(Get(obj), value);

        const int newValue = 1122334455;
        value = newValue;
        Assert.Equal(newValue, Get(obj));

        static int Get(TestModel obj)
        {
            return (int)typeof(TestModel).GetRequiredField("PrivateField").GetValue(obj)!;
        }
    }

    [ILAccessor(ILAccessorKind.Field, Name = "_message")]
    public static extern ref string Message(Exception obj);

    [Fact]
    public void RefPrivateField_CrossAssembly_Get_Set()
    {
        var ex = new Exception("xxxxxx");
        ref var value = ref Message(ex);
        Assert.Equal(ex.Message, value);

        var newValue = ex.Message + "_Modified";
        value = newValue;
        Assert.Equal(newValue, ex.Message);
    }
}