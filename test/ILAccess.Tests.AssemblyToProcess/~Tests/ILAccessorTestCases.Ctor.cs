// ReSharper disable ConvertToConstant.Local
namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [FakeFact]
    public void Ctor_NoParams()
    {
        var obj = TestModelAccessors.Ctor();
        Assert.NotNull(obj);

        Assert.NotEqual(default, obj._d);
        Assert.NotEqual(default, obj._i);
        Assert.NotEqual(default, obj._s);
    }

    [FakeFact]
    public void Ctor_WithParams()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = TestModelAccessors.Ctor(i, s, ref d);
        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }

    [FakeFact]
    public void Ctor_AsMethod()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = RuntimeHelpers.GetUninitializedObject<TestModel>();
        obj.CtorAsMethod(i, s, ref d);

        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }

    [FakeFact]
    public void Ctor_NoParams_CrossAssembly_Exception()
    {
        var ex = ExceptionAccessors.Ctor();
        Assert.NotNull(ex);
        Assert.Empty(ex.Message);
    }

    [FakeFact]
    public void Ctor_WithParams_CrossAssembly_Exception()
    {
        var message = "An error occurred.";
        var ex = ExceptionAccessors.Ctor(message);
        Assert.NotNull(ex);
        Assert.Equal(message, ex.Message);
    }
}
