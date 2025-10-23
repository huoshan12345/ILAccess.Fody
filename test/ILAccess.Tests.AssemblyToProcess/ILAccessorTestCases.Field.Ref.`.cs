using System;

namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void RefPublicStaticField_T_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPublicStaticField();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPublicStaticField());

        static string? Get()
        {
            return GenericTestModel<string>.PublicStaticField;
        }

        static void Set(string value)
        {
            GenericTestModel<string>.PublicStaticField = value;
        }
    }

    [Fact]
    public void RefPrivateStaticField_T_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPrivateStaticField();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPrivateStaticField());

        static string? Get()
        {
            return (string?)typeof(GenericTestModel<string>).GetRequiredField("PrivateStaticField").GetValue(null);
        }

        static void Set(string? value)
        {
            typeof(GenericTestModel<string>).GetRequiredField("PrivateStaticField").SetValue(null, value);
        }
    }

    [Fact]
    public void RefPublicField_T_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string> { PublicField = random.NextString(10) };
        ref var value = ref obj.RefPublicField();
        Assert.Equal(obj.PublicField, value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, obj.PublicField);
        Assert.Equal(newValue, obj.RefPublicField());
    }

    [Fact]
    public void RefPrivateField_T_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(obj, random.NextString(10));
        ref var value = ref obj.RefPrivateField();
        Assert.Equal(Get(obj), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get(obj));
        Assert.Equal(newValue, obj.RefPrivateField());

        static string? Get(GenericTestModel<string> obj)
        {
            return (string?)typeof(GenericTestModel<string>).GetRequiredField("PrivateField").GetValue(obj);
        }

        static void Set(GenericTestModel<string> obj, string? value)
        {
            typeof(GenericTestModel<string>).GetRequiredField("PrivateField").SetValue(obj, value);
        }
    }
}
