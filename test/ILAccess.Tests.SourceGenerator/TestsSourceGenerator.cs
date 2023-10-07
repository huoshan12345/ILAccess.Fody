using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using ILAccess.Tests.SourceGenerator.Sources;
using Microsoft.CodeAnalysis;

namespace ILAccess.Tests.SourceGenerator;

[Generator]
public class TestsSourceGenerator : ISourceGenerator
{
    public static readonly string[] Namespaces =
    {
        "ILAccess",
        "Tests",
        "AssemblyToProcess"
    };

    public static readonly string AssemblyName = string.Join(".", Namespaces);

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            ExecuteInternal(context);
        }
        catch (Exception e)
        {
            //This is temporary till https://github.com/dotnet/roslyn/issues/46084 is fixed
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "SI0000",
                    "An exception was thrown by the StrongInject generator",
                    "An exception was thrown by the StrongInject generator: '{0}'",
                    "StrongInject",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                Location.None,
                e.ToString()));
        }
    }

    //By not inlining we make sure we can catch assembly loading errors when jitting this method
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ExecuteInternal(GeneratorExecutionContext context)
    {
        var assemblySymbol = context.Compilation.SourceModule.ReferencedAssemblySymbols.FirstOrDefault(q => q.Name == AssemblyName);
        if (assemblySymbol == null)
            throw new InvalidOperationException("Cannot find assembly symbol: " + AssemblyName);

        var cur = Namespaces.Aggregate(assemblySymbol.GlobalNamespace, (current, ns) => current.GetNamespaceMembers().First(m => m.Name == ns));
        var types = cur.GetTypeMembers();

        foreach (var type in types)
        {
            var idx = type.Name.IndexOf("TestCases", StringComparison.Ordinal);
            if (idx < 0)
                continue;

            var className = type.Name.Substring(0, idx) + "Tests";
            var (name, code) = TestsSource.Generate(type, className);
            context.AddSource(name, code);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        //#if DEBUG
        //            if (!Debugger.IsAttached)
        //            {
        //                Debugger.Launch();
        //            }
        //            Debug.WriteLine("Initalize code generator");
        //#endif
    }
}
