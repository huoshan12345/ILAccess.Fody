using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA2211
#pragma warning disable IDE0051
#pragma warning disable IDE0044
#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace ILAccess.Tests.AssemblyToProcess;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
public class TestModel
{
    private static int PrivateStaticField = 1;
    public static int PublicStaticField = 1;
    private int PrivateField = 1;
    public int PublicField = 1;

    private static readonly int PrivateStaticReadonlyField = 1;
    public static readonly int PublicStaticReadonlyField = 1;
    private readonly int PrivateReadonlyField = 1;
    public readonly int PublicReadonlyField = 1;

    private static int PrivateStaticProperty { get; set; } = 1;
    public static int PublicStaticProperty { get; set; } = 1;
    private int PrivateProperty { get; set; } = 1;
    public int PublicProperty { get; set; } = 1;

    public int PublicPropertyWithPrivateSetter { get; private set; } = 1;
    public int PublicPropertyWithPrivateGetter { private get; set; } = 1;
    public int PublicPropertyWithoutSetter { get; } = 1;
}
