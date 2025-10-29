namespace ILAccess.Tests.AssemblyToProcess;

partial class ILAccessorTestCases
{
    [FakeFact]
    public void GenericPrivateMethod()
    {
        var obj = new TestModel();
        {
            var result = obj.GetString("x");
            Assert.Equal("x", result);
        }
        {
            int? input = 5;
            var result = obj.GetString(input);
            Assert.Equal("5", result);
        }
        {
            var result = obj.GetString("x", 5);
            Assert.Equal("x5", result);
        }
    }
}
