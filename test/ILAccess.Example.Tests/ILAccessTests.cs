using System.Collections;
using System.Reflection;
using FclEx.Extensions;
using Fody;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xunit;
using Xunit.Abstractions;

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

public class ILAccessTests(ITestOutputHelper output)
{
    private const string ProjectName = "ILAccess.Example";

    private const string AssemblyExtension =
#if NETFRAMEWORK
         ".exe";
#else
         ".dll";
#endif
    private const string AssemblyName = ProjectName + AssemblyExtension;

    [Fact(
        Skip = $"set DisableFody for the {ProjectName} first and then run this test"
        )]
    public void Weave_Test()
    {
        var rootDir = AppContext.BaseDirectory.TakeUntil("test", false);
        var projectDir = Path.Combine(rootDir, "src", ProjectName);
        var projectFile = Path.Combine(projectDir, $"{ProjectName}.csproj");
        var assemblyPath = Path.Combine(AppContext.BaseDirectory, AssemblyName);
        var weaver = Path.Combine(AppContext.BaseDirectory, "ILAccess.Fody.dll");

        var assemblyBytes = File.ReadAllBytes(assemblyPath);
        var assembly = Assembly.Load(assemblyBytes);
        // var refs = assembly.GetAllReferenceAssemblies().Select(m => m.Location).OrderBy(m => m).ToArray();

        var refsFromText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "refs.txt"))
            .SplitToLines()
            .OrderBy(m => m)
            .ToArray();

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
            References = refsFromText.JoinWith(";"),
            ReferenceCopyLocalFiles = [],
            RuntimeCopyLocalFiles = [],
            WeaverFiles = [new TaskItem(weaver)],
            WeaverConfiguration = null,
            PackageReferences = [],
            NCrunchOriginalSolutionDirectory = null,
            SolutionDirectory = rootDir,
            DefineConstants = null,
            IntermediateCopyLocalFilesCache = Path.Combine(AppContext.BaseDirectory, $"{ProjectName}.Fody.CopyLocal.cache"),
            RuntimeCopyLocalFilesCache = Path.Combine(AppContext.BaseDirectory, $"{ProjectName}.Fody.RuntimeCopyLocal.cache"),
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