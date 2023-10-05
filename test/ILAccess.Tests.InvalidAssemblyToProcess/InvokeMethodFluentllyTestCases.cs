using System;
using System.Collections.Generic;
using System.Text;
using ILAccess.Tests.AssemblyToProcess;

namespace ILAccess.Tests.InvalidAssemblyToProcess
{
    public class InvokeMethodFluentllyTestCases
    {
        private class InheritIDefaultMethod : IHasDefaultMethod
        {
            public string Method(int x, string y) => throw new InvalidOperationException();
        }

        public string InvokeMethod_WithVariable()
        {
            var obj = new InheritIDefaultMethod().Base<IHasDefaultMethod>();
            return obj.Method(0, string.Empty);
        }

        public string InvokeMethod_NotDeclaredByInterface()
        {
            var obj = new InheritIDefaultMethod();
            return obj.Base<IHasDefaultMethod>().ToString();
        }
    }
}
