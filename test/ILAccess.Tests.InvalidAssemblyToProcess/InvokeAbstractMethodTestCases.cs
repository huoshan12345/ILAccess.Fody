using System;
using System.Collections.Generic;
using System.Text;
using ILAccess.Tests.AssemblyToProcess;

namespace ILAccess.Tests.InvalidAssemblyToProcess
{
    public class InvokeAbstractMethodTestCases
    {
        private class InheritIEmptyMethod : IHasEmptyMethod
        {
            public string Property => throw new InvalidOperationException();
            public string Method(int x, string y) => throw new InvalidOperationException();
        }

        private class InheritIOverridedMethod : IHasOverridedMethod
        {
            public string Method(int x, string y) => throw new InvalidOperationException();
        }

        public string EmptyMethod_Invoke()
        {
            var obj = new InheritIEmptyMethod();
            return obj.Base<IHasEmptyMethod>().Method(0, string.Empty);
        }

        public string EmptyMethod_Invoke_MultiLevel()
        {
            var obj = new InheritIOverridedMethod();
            return obj.Base<IHasEmptyMethod>().Method(0, string.Empty);
        }
    }
}
