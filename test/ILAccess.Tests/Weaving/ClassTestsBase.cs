using System.Linq;
using System.Reflection;
using ILAccess.Tests.InvalidAssemblyToProcess;

namespace ILAccess.Tests.Weaving;

public abstract class ClassTestsBase
{
    protected static readonly string VerifiableAssembly = typeof(AssemblyToProcessReference).Assembly.GetName().Name!;
    protected static readonly string InvalidAssembly = typeof(InvalidAssemblyToProcessReference).Assembly.GetName().Name!;

    protected virtual bool NetStandard => false;
    protected abstract string ClassName { get; }

    protected dynamic GetInstance()
    {
        var testResult = NetStandard
            ? StandardAssemblyToProcessFixture.TestResult
            : AssemblyToProcessFixture.TestResult;

        var allTypes = testResult.Assembly.GetTypes();
        var types = allTypes.Where(m => m.Name == ClassName).ToArray();
        return types.Length switch
        {
            0 => throw new InvalidOperationException($"Cannot find type '{ClassName}' in assembly '{testResult.Assembly.GetName().Name}'."),
            1 => testResult.GetInstance(types[0].FullName!),
            _ => throw new InvalidOperationException($"Found more than 1 type named '{ClassName}' in assembly '{testResult.Assembly.GetName().Name}'."),
        };
    }

    protected string ShouldHaveError(string methodName)
        => InvalidAssemblyToProcessFixture.ShouldHaveError($"{InvalidAssembly}.{ClassName}", methodName, true);
}