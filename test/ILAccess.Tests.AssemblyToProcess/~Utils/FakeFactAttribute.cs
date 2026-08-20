using System;

namespace ILAccess.Tests.AssemblyToProcess;

/// <summary>
/// Marks a test method as a fake fact to help SourceGenerator to generate the actual test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FakeFactAttribute : FactAttribute
{
    public FakeFactAttribute()
    {
        Skip = "This is a fake fact attribute for source generator.";
    }
}
