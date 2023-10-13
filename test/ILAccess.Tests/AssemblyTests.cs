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

    [Fact]
    public void ShouldNotAddReferenceToPrivateCoreLib()
    {
        AssemblyToProcessFixture.ResultModule.AssemblyReferences.ShouldNotContain(i => i.Name == "System.Private.CoreLib");
        StandardAssemblyToProcessFixture.ResultModule.AssemblyReferences.ShouldNotContain(i => i.Name == "System.Private.CoreLib");
        InvalidAssemblyToProcessFixture.ResultModule.AssemblyReferences.ShouldNotContain(i => i.Name == "System.Private.CoreLib");
    }
}