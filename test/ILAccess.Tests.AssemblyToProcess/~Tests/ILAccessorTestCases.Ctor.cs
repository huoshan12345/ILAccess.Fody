// ReSharper disable ConvertToConstant.Local

using System.Runtime.Serialization;

#pragma warning disable SYSLIB0050 // Formatter-based serialization is obsolete

namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [FakeFact]
    public void Ctor_NoParams()
    {
        var obj = Accessors.Ctor();
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
        var obj = Accessors.Ctor(i, s, ref d);
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
        var obj = (TestModel)FormatterServices.GetUninitializedObject(typeof(TestModel));
        obj.PrivateCtorAsMethod(i, s, ref d);

        Assert.NotNull(obj);
        Assert.Equal(i, obj._i);
        Assert.Equal(s, obj._s);
        Assert.Equal(d, obj._d);
    }
}
