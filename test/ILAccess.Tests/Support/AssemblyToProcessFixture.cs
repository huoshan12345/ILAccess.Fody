using System.Collections.Generic;
using System.Text;
using Fody;
using ILAccess.Fody.Extensions;
using ILAccess.Fody.Processing;
using ILAccess.Tests.AssemblyToProcess;
using Mono.Cecil;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;
using SequencePoint = Mono.Cecil.Cil.SequencePoint;

#pragma warning disable 618

namespace ILAccess.Tests.Support;

public static class AssemblyToProcessFixture
{
    public static TestResult TestResult { get; }

    public static ModuleDefinition OriginalModule { get; }
    public static ModuleDefinition ResultModule { get; }

    static AssemblyToProcessFixture()
    {
        (TestResult, OriginalModule, ResultModule) = Process<AssemblyToProcessReference>();
    }

    internal static (TestResult testResult, ModuleDefinition originalModule, ModuleDefinition resultModule) Process<T>()
    {
        var assemblyPath = FixtureHelper.IsolateAssembly<T>();

        var weavingTask = new GuardedWeaver();

        var testResult = weavingTask.ExecuteTestRun(
            assemblyPath,
            ignoreCodes: new[]
            {
                "0x801312da" // VLDTR_E_MR_VARARGCALLINGCONV
            },
            writeSymbols: true,
            beforeExecuteCallback: BeforeExecuteCallback,
            runPeVerify: false
        );

        using var assemblyResolver = new TestAssemblyResolver();

        var readerParams = new ReaderParameters(ReadingMode.Immediate)
        {
            ReadSymbols = true,
            AssemblyResolver = assemblyResolver
        };

        var originalModule = ModuleDefinition.ReadModule(assemblyPath, readerParams);
        var resultModule = ModuleDefinition.ReadModule(testResult.AssemblyPath, readerParams);

        return (testResult, originalModule, resultModule);
    }

    internal static void BeforeExecuteCallback(ModuleDefinition module)
    {
        // This reference is added by Fody, it's not supposed to be there
        module.AssemblyReferences.RemoveWhere(m => m.Name == AssemblyNames.CoreLib);
    }

    internal class GuardedWeaver : ModuleWeaver
    {
        private readonly List<string> _errors = new();

        public override void Execute()
        {
            try
            {
                base.Execute();
            }
            catch (Exception ex)
            {
                var str = new StringBuilder();
                foreach (var error in _errors)
                    str.AppendLine(error);

                str.AppendLine(ex.Message);
                throw new InvalidOperationException(str.ToString());
            }
        }

        protected override void AddError(string message, SequencePoint? sequencePoint)
        {
            _errors.Add(message);
            base.AddError(message, sequencePoint);
        }
    }
}