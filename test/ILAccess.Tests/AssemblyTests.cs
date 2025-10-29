using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ILAccess.Fody.Processing;
using MoreFodyHelpers;

namespace ILAccess.Tests;

public class AssemblyTests
{
    [Fact]
    public void ShouldNotReferenceValueTuple()
    {
        // System.ValueTuple may cause issues in some configurations, avoid using it.

        using var fileStream = File.OpenRead(typeof(ModuleWeaver).Assembly.Location);
        using var peReader = new PEReader(fileStream);
        var metadataReader = peReader.GetMetadataReader();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var typeRefHandle in metadataReader.TypeReferences)
        {
            var typeRef = metadataReader.GetTypeReference(typeRefHandle);

            var typeNamespace = metadataReader.GetString(typeRef.Namespace);
            if (typeNamespace != typeof(ValueTuple).Namespace)
                continue;

            var typeName = metadataReader.GetString(typeRef.Name);
            typeName.ShouldNotContain(nameof(ValueTuple));
        }
    }

    [Fact(Skip = "no longer valid")]
    public void ShouldNotAddReferenceToPrivateCoreLib()
    {
        var modules = new[]
        {
            AssemblyToProcessFixture.ResultModule,
            StandardAssemblyToProcessFixture.ResultModule,
            InvalidAssemblyToProcessFixture.ResultModule,
        };

        foreach (var module in modules)
        {
            module.AssemblyReferences.ShouldNotContain(m => m.Name == AssemblyNames.SystemPrivateCoreLib);
        }
    }
}