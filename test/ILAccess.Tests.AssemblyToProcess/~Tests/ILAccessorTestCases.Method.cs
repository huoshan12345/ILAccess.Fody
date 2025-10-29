using System;
using System.Reflection;

namespace ILAccess.Tests.AssemblyToProcess;

partial class ILAccessorTestCases
{
    [FakeFact]
    public void PrivateMethod()
    {
        var obj = new TestModel();
        {
            var result = obj.Plus(1, "xxxx");
            Assert.Equal((obj._i + 1, obj._s + "xxxx"), result);
        }
        {
            var result = obj.Plus(1, 1.0);
            Assert.Equal((obj._i + 1, obj._d + 1.0), result);
        }
    }

    [FakeFact]
    public void StaticPrivateMethod()
    {
        var result = TestModelAccessors.GetStart(null);
        Assert.Equal(TestModel._start, result);
    }

    [FakeFact]
    public void PrivateMethod_CrossAssembly_Exception()
    {
        var ex = new Exception("xxxxxxxx");
        var result = ex.GetClassName();
        Assert.Equal("System.Exception", result);
    }

    [FakeFact]
    public void StaticPrivateMethod_CrossAssembly_Exception()
    {
        var ex = new Exception("xxxxxxxx");
        var result = ExceptionAccessors.IsImmutableAgileException(null, ex);
        Assert.False(result);
    }
}

