using System.Collections;
using System.Reflection;
using FclEx.Extensions;
using Fody;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xunit;

namespace ILAccess.Example.Tests;

public static class Extensions
{
    public static IEnumerable<Assembly> GetAllReferenceAssemblies(this Assembly source)
    {
        var results = new List<Assembly> { source };
        foreach (var name in source.GetReferencedAssemblies())
        {
            var loaded = Assembly.Load(name);
            results.AddRange(loaded.GetAllReferenceAssemblies());
        }
        return results.Distinct();
    }
}

public class ILAccessTests
{
    [Fact]
    public void Weave_Test()
    {
        const string projectName = "ILAccess.Example";
        var rootDir = AppContext.BaseDirectory.TakeUntil("test", false);
        var projectDir = Path.Combine(rootDir, "src", projectName);
        var projectFile = Path.Combine(projectDir, $"{projectName}.csproj");
        var assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{projectName}.dll");
        var weaver = Path.Combine(AppContext.BaseDirectory, "ILAccess.Fody.dll");

        var assembly = Assembly.LoadFile(assemblyPath);
        var refs = assembly.GetAllReferenceAssemblies().Select(m => m.Location).JoinWith(";");

        var task = new WeavingTask
        {
            AssemblyFile = assemblyPath,
            IntermediateDirectory = AppContext.BaseDirectory,
            KeyOriginatorFile = null,
            AssemblyOriginatorKeyFile = null,
            SignAssembly = false,
            DelaySign = false,
            ProjectDirectory = projectDir,
            ProjectFile = projectFile,
            DocumentationFile = null,
            References = refs,
            ReferenceCopyLocalFiles = [],
            RuntimeCopyLocalFiles = [],
            WeaverFiles = [new TaskItem(weaver)],
            WeaverConfiguration = null,
            PackageReferences = [],
            NCrunchOriginalSolutionDirectory = null,
            SolutionDirectory = rootDir,
            DefineConstants = null,
            IntermediateCopyLocalFilesCache = null,
            RuntimeCopyLocalFilesCache = null,
            GenerateXsd = false,
            TreatWarningsAsErrors = false,
            BuildEngine = new FakeBuildEngine(),
        };

        var result = task.Execute();
        Assert.True(result);
    }
}


public class FakeBuildEngine : IBuildEngine
{
    public readonly List<BuildErrorEventArgs> LogErrorEvents = [];
    public readonly List<BuildMessageEventArgs> LogMessageEvents = [];
    public readonly List<CustomBuildEventArgs> LogCustomEvents = [];
    public readonly List<BuildWarningEventArgs> LogWarningEvents = [];

    public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
    {
        throw new NotImplementedException();
    }

    public int ColumnNumberOfTaskNode => 0;
    public bool ContinueOnError => throw new NotImplementedException();
    public int LineNumberOfTaskNode => 0;
    public string ProjectFileOfTaskNode => "fake ProjectFileOfTaskNode";

    public void LogCustomEvent(CustomBuildEventArgs e) => LogCustomEvents.Add(e);
    public void LogErrorEvent(BuildErrorEventArgs e) => LogErrorEvents.Add(e);
    public void LogMessageEvent(BuildMessageEventArgs e) => LogMessageEvents.Add(e);
    public void LogWarningEvent(BuildWarningEventArgs e) => LogWarningEvents.Add(e);
}