using System.Linq;
using System.Reflection;
using ILAccess.Fody.Processing;
using ILAccess.Tests.InvalidAssemblyToProcess;

namespace ILAccess.Tests.Support;

public static class InvalidAssemblyToProcessFixture
{
    public static TestResult TestResult { get; }

    public static ModuleDefinition ResultModule { get; }

    public static bool IsDebug { get; }

    static InvalidAssemblyToProcessFixture()
    {
        var weavingTask = new ModuleWeaver();
        TestResult = weavingTask.ExecuteTestRun(
            FixtureHelper.IsolateAssembly<InvalidAssemblyToProcessReference>(),
            false,
            beforeExecuteCallback: AssemblyToProcessFixture.BeforeExecuteCallback
        );

        using var assemblyResolver = new TestAssemblyResolver();

        ResultModule = ModuleDefinition.ReadModule(TestResult.AssemblyPath, new ReaderParameters(ReadingMode.Immediate)
        {
            AssemblyResolver = assemblyResolver
        });

        var typeName = TestResult.Assembly.GetName().Name + "." + nameof(InvalidAssemblyToProcessReference);
        IsDebug = (bool)TestResult.Assembly.GetType(typeName, true)!
            .InvokeMember(name: nameof(IsDebug),
                invokeAttr: BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
                binder: null,
                target: null,
                args: null)!;
    }

    public static string ShouldHaveError(string className, string methodName, bool sequencePointRequired)
    {
        var expectedMessagePart = $" {className}::{methodName}(";
        var errorMessage = TestResult.Errors.SingleOrDefault(err => err.Text.Contains(expectedMessagePart));
        errorMessage.ShouldNotBeNull();

        if (sequencePointRequired)
            errorMessage!.SequencePoint.ShouldNotBeNull();

        return errorMessage!.Text;
    }
}