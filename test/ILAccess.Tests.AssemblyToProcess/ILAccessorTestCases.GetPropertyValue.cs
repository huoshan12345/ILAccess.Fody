namespace ILAccess.Tests.AssemblyToProcess;

public partial class ILAccessorTestCases
{
    [Fact]
    public void Public_Static_Property_Test()
    {
        var obj = new TestModel();
        var value = obj.ILAccess().GetPropertyValue<int>("PublicStaticProperty");
        Assert.Equal(1, value);
    }
}
