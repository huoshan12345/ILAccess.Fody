using ILAccess.Tests.InvalidAssemblyToProcess;
using ILAccess.Tests.Support;
using Xunit;

namespace ILAccess.Tests.Weaving
{
    public class InvokeMethodFluentllyTests : ClassTestsBase
    {
        protected override string ClassName => nameof(InvokeMethodFluentllyTestCases);

        [InvalidAssemblyOnlyDebugFact]
        public void InvokeMethod_WithVariable()
        {
            var error = ShouldHaveError(nameof(InvokeMethodFluentllyTestCases.InvokeMethod_WithVariable));
            error.ShouldContain("The method Base<T> requires that an interface methods to be base-invoked with its return value fluently");
        }

        [Fact]
        public void InvokeMethod_NotDeclaredByInterface()
        {
            var error = ShouldHaveError(nameof(InvokeMethodFluentllyTestCases.InvokeMethod_NotDeclaredByInterface));
            error.ShouldContain("The method Base<T> requires that an interface methods to be base-invoked with its return value fluently");
        }
    }
}
