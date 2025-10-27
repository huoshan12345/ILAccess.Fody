using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fody;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;
using Microsoft.Build.Utilities;
using Xunit;
using Xunit.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace ILAccess.Example.Tests;

public class Startup
{
    [ModuleInitializer]
    public static void Init()
    {
        var instance = MSBuildLocator.QueryVisualStudioInstances()
            .OrderByDescending(instance => instance.Version)
            .First();
        // This method must be called, otherwise it will prompt that the msbuild file cannot be found
        MSBuildLocator.RegisterInstance(instance);
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

    private static string GetReferencesPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "refs.txt";
        }
        else
        {
            return "refs-linux.txt";
        }
    }


    [Fact]
    public void Weave_Test()
    {
        var rootDir = AppContext.BaseDirectory.TakeUntil("test", false);
        var projectDir = Path.Combine(rootDir, "src", ProjectName);
        var projectFile = Path.Combine(projectDir, $"{ProjectName}.csproj");
        var projectRootElement = ProjectRootElement.Open(projectFile);
        var project = new Project(projectRootElement);

        if (project.GetPropertyValue("DisableFody") != "true")
        {
            project.SetProperty("DisableFody", "true");
        }

        //var buildLogger = new BuildLogger();
        var buildResult = project.Build();
        Assert.True(buildResult);

        var assemblyPath = Path.Combine(AppContext.BaseDirectory, AssemblyName);
        var weaver = Path.Combine(AppContext.BaseDirectory, "ILAccess.Fody.dll");

        //var assemblyBytes = File.ReadAllBytes(assemblyPath);
        //var assembly = Assembly.Load(assemblyBytes);
        // var refs = assembly.GetAllReferenceAssemblies().Select(m => m.Location).OrderBy(m => m).ToArray();

        var refsFromText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TestData", GetReferencesPath()))
            .Split(['\r', '\n'])
            .Select(m => m.Trim())
            .Where(m => m.Length > 0)
            .OrderBy(m => m)
            .ToArray();

        //var engine = new FakeBuildEngine();
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
            References = string.Join(";", refsFromText),
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
            //BuildEngine = engine,
        };

        var result = task.Execute();
        //var error = engine.LogErrorEvents.FirstOrDefault()?.Message;
        //Assert.True(result, error);
        //Assert.True(engine.LogErrorEvents.Count == 0, error);
    }
}

public class BuildLogger : ILogger
{
    public void Initialize(IEventSource eventSource)
    {
        eventSource.ErrorRaised += (sender, args) => Errors.Add(args);
    }

    public void Shutdown()
    {
        Errors.Clear();
    }

    public LoggerVerbosity Verbosity { get; set; }
    public string? Parameters { get; set; }
    public List<BuildErrorEventArgs> Errors = [];
}

//public class FakeBuildEngine : IBuildEngine
//{
//    public readonly List<BuildErrorEventArgs> LogErrorEvents = [];
//    public readonly List<BuildMessageEventArgs> LogMessageEvents = [];
//    public readonly List<CustomBuildEventArgs> LogCustomEvents = [];
//    public readonly List<BuildWarningEventArgs> LogWarningEvents = [];

//    public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties,
//        IDictionary targetOutputs)
//    {
//        throw new NotImplementedException();
//    }

//    public int ColumnNumberOfTaskNode => 0;
//    public bool ContinueOnError => throw new NotImplementedException();
//    public int LineNumberOfTaskNode => 0;
//    public string ProjectFileOfTaskNode => "fake ProjectFileOfTaskNode";

//    public void LogCustomEvent(CustomBuildEventArgs e) => LogCustomEvents.Add(e);
//    public void LogErrorEvent(BuildErrorEventArgs e) => LogErrorEvents.Add(e);
//    public void LogMessageEvent(BuildMessageEventArgs e) => LogMessageEvents.Add(e);
//    public void LogWarningEvent(BuildWarningEventArgs e) => LogWarningEvents.Add(e);
//}

file static class Extensions
{
    public static string TakeUntil(this string source, string separator, bool includeSeparator = true,
        StringComparison comparison = StringComparison.Ordinal, bool untilLast = false)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (separator == null) throw new ArgumentNullException(nameof(separator));

        var location = untilLast
            ? source.LastIndexOf(separator, comparison)
            : source.IndexOf(separator, comparison);

        if (location < 0)
            return source;

        if (includeSeparator)
            location += separator.Length;

        return source[..location];
    }

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