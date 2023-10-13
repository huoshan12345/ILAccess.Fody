using System;
#pragma warning disable IDE0051

namespace ILAccess.Example;

public class TestModel
{
    private static int PrivateStaticProperty { get; set; } = 1;
    private int PrivateProperty { get; set; } = 2;
}

internal class Program
{
    private static void Main(string[] args)
    {
        var obj = new TestModel();
        {
            var value = obj.ILAccess().GetPropertyValue<int>("PrivateProperty");
            Console.WriteLine("PrivateProperty: " + value);
        }
        {
            var value = obj.ILAccess().GetPropertyValue<int>("PrivateStaticProperty");
            Console.WriteLine("PrivateStaticProperty: " + value);
        }

        Console.Read();
    }
}
