using ILAccess.Tests.InvalidAssemblyToProcess;
using ILAccess.Tests.Support;
using Xunit;

namespace ILAccess.Tests.Weaving
{
    public class AnchorMethodTests : ClassTestsBase
    {
        protected override string ClassName => nameof(AnchorMethodTestCases);

        [Fact]
        public void Base_WithSelf()
        {
            var error = ShouldHaveError(nameof(AnchorMethodTestCases.Base_WithSelf));
            error.ShouldContain("The method Base<T> requires that T is an interface type");
        }

        [Fact]
        public void Base_WithParent()
        {
            var error = ShouldHaveError(nameof(AnchorMethodTestCases.Base_WithParent));
            error.ShouldContain("The method Base<T> requires that T is an interface type");
        }

        [Fact]
        public void Base_MoreThanOnce()
        {
            var error = ShouldHaveError(nameof(AnchorMethodTestCases.Base_MoreThanOnce));
            error.ShouldContain("The method Base<T> cannot be invoked followed by another Base<T>");
        }
    }
}
