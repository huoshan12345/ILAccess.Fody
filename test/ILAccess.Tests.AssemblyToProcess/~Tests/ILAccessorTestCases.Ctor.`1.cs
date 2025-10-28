// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedMember.Global

namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [FakeFact]
    public void GenericType_Ctor_NoParams()
    {
        var obj = TestModelAccessors.Ctor<string>();
        Assert.NotNull(obj);

        Assert.NotEqual(default, obj._d);
        Assert.NotEqual(default, obj._i);
        Assert.NotEqual(default, obj._s);
    }

    [FakeFact]
    public void GenericType_Ctor_WithParams()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = TestModelAccessors.Ctor<string>(i, s, ref d);
        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }

    [FakeFact]
    public void GenericType_Ctor_AsMethod()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = RuntimeHelpers.GetUninitializedObject<TestModel<string>>();
        obj.CtorAsMethod(i, s, ref d);

        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }

    [FakeFact]
    public void GenericType_Ctor_GenericAccessors_NoParams()
    {
        var obj = GenericTestModelAccessors<string>.Ctor();
        Assert.NotNull(obj);

        Assert.NotEqual(default, obj._d);
        Assert.NotEqual(default, obj._i);
        Assert.NotEqual(default, obj._s);
    }

    [FakeFact]
    public void GenericType_Ctor_GenericAccessors_WithParams()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = GenericTestModelAccessors<string>.Ctor(i, s, ref d);
        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }

    [FakeFact]
    public void GenericType_Ctor_GenericAccessors_AsMethod()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = RuntimeHelpers.GetUninitializedObject<TestModel<string>>();
        GenericTestModelAccessors<string>.CtorAsMethod(obj, i, s, ref d);

        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }

    [FakeFact]
    public void GenericType_CrossAssembly_List_Ctor_NoParams()
    {
        var obj = ListAccessors.Ctor<string>();
        Assert.NotNull(obj);
    }

    [FakeFact]
    public void GenericType_CrossAssembly_List_Ctor_WithParams()
    {
        var obj = ListAccessors.Ctor<string>(5);
        Assert.NotNull(obj);
        Assert.Equal(5, obj.Capacity);
    }
}
