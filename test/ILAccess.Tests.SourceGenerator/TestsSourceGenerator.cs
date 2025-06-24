using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using ILAccess.Tests.SourceGenerator.Sources;

namespace ILAccess.Tests.SourceGenerator;

[Generator]
public class TestsSourceGenerator : IIncrementalGenerator
{
    public static readonly string[] Namespaces =
    [
        "ILAccess",
        "Tests",
        "AssemblyToProcess",
    ];

    public static readonly string AssemblyName = string.Join(".", Namespaces);
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (Debugger.IsAttached == false)
        //    Debugger.Launch();

        context.RegisterImplementationSourceOutput(context.CompilationProvider, (ctx, provider) =>
        {
            var assemblySymbol = provider.SourceModule.ReferencedAssemblySymbols.FirstOrDefault(q => q.Name == AssemblyName);
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
                ctx.AddSource(name, code);
            }
        });
    }
}
