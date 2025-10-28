using System;

namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [FakeFact]
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

    [FakeFact]
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

    [FakeFact]
    public void PublicField_T_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string> { PublicField = random.NextString(10) };
        var value = obj.PublicField();
        Assert.Equal(obj.PublicField, value);
    }

    [FakeFact]
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

    [FakeFact]
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

    [FakeFact]
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

    [FakeFact]
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

    [FakeFact]
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


    [FakeFact]
    public void PublicStaticField_T_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        var value = obj.PublicStaticField_GenericAccessors();
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

    [FakeFact]
    public void PrivateStaticField_T_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        var value = obj.PrivateStaticField_GenericAccessors();
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

    [FakeFact]
    public void PublicField_T_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string> { PublicField = random.NextString(10) };
        var value = obj.PublicField_GenericAccessors();
        Assert.Equal(obj.PublicField, value);
    }

    [FakeFact]
    public void PrivateField_T_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(obj, random.NextString(10));
        var value = obj.PrivateField_GenericAccessors();
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
    
    [FakeFact]
    public void RefPublicStaticField_T_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPublicStaticField_GenericAccessors();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPublicStaticField_GenericAccessors());

        static string? Get()
        {
            return GenericTestModel<string>.PublicStaticField;
        }

        static void Set(string value)
        {
            GenericTestModel<string>.PublicStaticField = value;
        }
    }

    [FakeFact]
    public void RefPrivateStaticField_T_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPrivateStaticField_GenericAccessors();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPrivateStaticField_GenericAccessors());

        static string? Get()
        {
            return (string?)typeof(GenericTestModel<string>).GetRequiredField("PrivateStaticField").GetValue(null);
        }

        static void Set(string? value)
        {
            typeof(GenericTestModel<string>).GetRequiredField("PrivateStaticField").SetValue(null, value);
        }
    }

    [FakeFact]
    public void RefPublicField_T_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string> { PublicField = random.NextString(10) };
        ref var value = ref obj.RefPublicField_GenericAccessors();
        Assert.Equal(obj.PublicField, value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, obj.PublicField);
        Assert.Equal(newValue, obj.RefPublicField_GenericAccessors());
    }

    [FakeFact]
    public void RefPrivateField_T_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new GenericTestModel<string>();
        Set(obj, random.NextString(10));
        ref var value = ref obj.RefPrivateField_GenericAccessors();
        Assert.Equal(Get(obj), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get(obj));
        Assert.Equal(newValue, obj.RefPrivateField_GenericAccessors());

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
