using System;

namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void PublicStaticField_T_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        var value = obj.PublicStaticField();
        Assert.Equal(Get(), value);

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
    public void PrivateStaticField_T_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        var value = obj.PrivateStaticField();
        Assert.Equal(Get(), value);

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
    public void PublicField_T_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string> { PublicField = random.NextString(10) };
        var value = obj.PublicField();
        Assert.Equal(obj.PublicField, value);
    }

    [Fact]
    public void PrivateField_T_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(obj, random.NextString(10));
        var value = obj.PrivateField();
        Assert.Equal(Get(obj), value);

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
