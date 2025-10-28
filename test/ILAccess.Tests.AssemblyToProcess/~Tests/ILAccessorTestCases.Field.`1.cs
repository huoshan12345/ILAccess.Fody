using System;
using System.Collections.Generic;

namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [FakeFact]
    public void GenericType_PublicStaticField_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        var value = obj.PublicStaticField();
        Assert.Equal(Get(), value);

        static string? Get()
        {
            return TestModel<string>.PublicStaticField;
        }

        static void Set(string value)
        {
            TestModel<string>.PublicStaticField = value;
        }
    }

    [FakeFact]
    public void GenericType_PrivateStaticField_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        var value = obj.PrivateStaticField();
        Assert.Equal(Get(), value);

        static string? Get()
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateStaticField").GetValue(null);
        }

        static void Set(string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateStaticField").SetValue(null, value);
        }
    }

    [FakeFact]
    public void GenericType_PublicField_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string> { PublicField = random.NextString(10) };
        var value = obj.PublicField();
        Assert.Equal(obj.PublicField, value);
    }

    [FakeFact]
    public void GenericType_PrivateField_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(obj, random.NextString(10));
        var value = obj.PrivateField();
        Assert.Equal(Get(obj), value);

        static string? Get(TestModel<string> obj)
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateField").GetValue(obj);
        }

        static void Set(TestModel<string> obj, string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateField").SetValue(obj, value);
        }
    }

    [FakeFact]
    public void GenericType_RefPublicStaticField_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPublicStaticField();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPublicStaticField());

        static string? Get()
        {
            return TestModel<string>.PublicStaticField;
        }

        static void Set(string value)
        {
            TestModel<string>.PublicStaticField = value;
        }
    }

    [FakeFact]
    public void GenericType_RefPrivateStaticField_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPrivateStaticField();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPrivateStaticField());

        static string? Get()
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateStaticField").GetValue(null);
        }

        static void Set(string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateStaticField").SetValue(null, value);
        }
    }

    [FakeFact]
    public void GenericType_RefPublicField_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string> { PublicField = random.NextString(10) };
        ref var value = ref obj.RefPublicField();
        Assert.Equal(obj.PublicField, value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, obj.PublicField);
        Assert.Equal(newValue, obj.RefPublicField());
    }

    [FakeFact]
    public void GenericType_RefPrivateField_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(obj, random.NextString(10));
        ref var value = ref obj.RefPrivateField();
        Assert.Equal(Get(obj), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get(obj));
        Assert.Equal(newValue, obj.RefPrivateField());

        static string? Get(TestModel<string> obj)
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateField").GetValue(obj);
        }

        static void Set(TestModel<string> obj, string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateField").SetValue(obj, value);
        }
    }

    [FakeFact]
    public void GenericType_PublicStaticField_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        var value = obj.PublicStaticField_GenericAccessors();
        Assert.Equal(Get(), value);

        static string? Get()
        {
            return TestModel<string>.PublicStaticField;
        }

        static void Set(string value)
        {
            TestModel<string>.PublicStaticField = value;
        }
    }

    [FakeFact]
    public void GenericType_PrivateStaticField_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        var value = obj.PrivateStaticField_GenericAccessors();
        Assert.Equal(Get(), value);

        static string? Get()
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateStaticField").GetValue(null);
        }

        static void Set(string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateStaticField").SetValue(null, value);
        }
    }

    [FakeFact]
    public void GenericType_PublicField_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string> { PublicField = random.NextString(10) };
        var value = obj.PublicField_GenericAccessors();
        Assert.Equal(obj.PublicField, value);
    }

    [FakeFact]
    public void GenericType_PrivateField_GenericAccessors_Get()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(obj, random.NextString(10));
        var value = obj.PrivateField_GenericAccessors();
        Assert.Equal(Get(obj), value);

        static string? Get(TestModel<string> obj)
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateField").GetValue(obj);
        }

        static void Set(TestModel<string> obj, string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateField").SetValue(obj, value);
        }
    }

    [FakeFact]
    public void GenericType_RefPublicStaticField_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPublicStaticField_GenericAccessors();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPublicStaticField_GenericAccessors());

        static string? Get()
        {
            return TestModel<string>.PublicStaticField;
        }

        static void Set(string value)
        {
            TestModel<string>.PublicStaticField = value;
        }
    }

    [FakeFact]
    public void GenericType_RefPrivateStaticField_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(random.NextString(10));
        ref var value = ref obj.RefPrivateStaticField_GenericAccessors();
        Assert.Equal(Get(), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get());
        Assert.Equal(newValue, obj.RefPrivateStaticField_GenericAccessors());

        static string? Get()
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateStaticField").GetValue(null);
        }

        static void Set(string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateStaticField").SetValue(null, value);
        }
    }

    [FakeFact]
    public void GenericType_RefPublicField_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string> { PublicField = random.NextString(10) };
        ref var value = ref obj.RefPublicField_GenericAccessors();
        Assert.Equal(obj.PublicField, value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, obj.PublicField);
        Assert.Equal(newValue, obj.RefPublicField_GenericAccessors());
    }

    [FakeFact]
    public void GenericType_RefPrivateField_GenericAccessors_Get_Set()
    {
        var random = new Random(0);
        var obj = new TestModel<string>();
        Set(obj, random.NextString(10));
        ref var value = ref obj.RefPrivateField_GenericAccessors();
        Assert.Equal(Get(obj), value);

        var newValue = random.NextString(10);
        value = newValue;
        Assert.Equal(newValue, Get(obj));
        Assert.Equal(newValue, obj.RefPrivateField_GenericAccessors());

        static string? Get(TestModel<string> obj)
        {
            return (string?)typeof(TestModel<string>).GetRequiredField("PrivateField").GetValue(obj);
        }

        static void Set(TestModel<string> obj, string? value)
        {
            typeof(TestModel<string>).GetRequiredField("PrivateField").SetValue(obj, value);
        }
    }

    [FakeFact]
    public void GenericType_CrossAssembly_List_RefPrivateField_Get_Set()
    {
        var list = new List<string> { "xxxxxxxxxxx" };
        ref var items = ref list.Items();
        Assert.Equal(list.Count, items.Length);
        Assert.Equal(list[0], items[0]);

        items[0] = "yyyyyyyyyyy";
        Assert.Equal(items[0], list[0]);

        items = ["zzzzzzzzzzz", "aaaaaaaaaaa"];
        Assert.Equal(2, list.Count);
        Assert.Equal(items[0], list[0]);
        Assert.Equal(items[1], list[1]);
    }
}
