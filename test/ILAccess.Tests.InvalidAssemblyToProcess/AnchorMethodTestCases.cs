using ILAccess.Tests.AssemblyToProcess;

namespace ILAccess.Tests.InvalidAssemblyToProcess
{
    public class AnchorMethodTestCases
    {
        public string Base_WithSelf()
        {
            var obj = new HasDefaultMethod();
            return obj.Base().Method(0, string.Empty);
        }

        public string Base_WithParent()
        {
            var obj = new DerivedHasDefaultMethod();
            return obj.Base<HasDefaultMethod>().Method(0, string.Empty);
        }

        public string Base_MoreThanOnce()
        {
            var obj = new HasDefaultMethod();
            return obj.Base<IHasDefaultMethod>().Base<IHasDefaultMethod>().Method(0, string.Empty);
        }
    }
}
