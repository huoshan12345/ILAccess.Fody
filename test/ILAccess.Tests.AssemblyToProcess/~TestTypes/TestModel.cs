using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0051
#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace ILAccess.Tests.AssemblyToProcess;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public class TestModel
{
    private static readonly int PrivateStaticField = 1;
    public static readonly int PublicStaticField = 1;
    private readonly int PrivateField = 1;
    public readonly int PublicField = 1;

    private static int PrivateStaticProperty { get; set; } = 1;
    public static int PublicStaticProperty { get; set; } = 1;
    private int PrivateProperty { get; set; } = 1;
    public int PublicProperty { get; set; } = 1;
    public int PublicPropertyWithPrivateSetter { get; private set; } = 1;
    public int PublicPropertyWithPrivateGetter { private get; set; } = 1;
    public int PublicPropertyWithoutSetter { get; } = 1;
}
