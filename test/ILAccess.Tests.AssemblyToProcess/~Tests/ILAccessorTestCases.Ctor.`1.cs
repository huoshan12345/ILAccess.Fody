// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedMember.Global

namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [FakeFact]
    public void Ctor_T_NoParams()
    {
        var obj = GenericAccessors<string>.Ctor();
        Assert.NotNull(obj);

        Assert.NotEqual(default, obj._d);
        Assert.NotEqual(default, obj._i);
        Assert.NotEqual(default, obj._s);
    }

    [FakeFact]
    public void Ctor_T_WithParams()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = GenericAccessors<string>.Ctor(i, s, ref d);
        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }

    [FakeFact]
    public void Ctor_T_AsMethod()
    {
        var i = 42;
        var s = "Hello, World!";
        var d = 3.14d;
        var obj = RuntimeHelpers.GetUninitializedObject<TestModel<string>>();
        obj.PrivateCtorAsMethod(i, s, ref d);

        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }
}
