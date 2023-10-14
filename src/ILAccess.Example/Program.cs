using System;

#pragma warning disable IDE0051

namespace ILAccess.Example;

public class TestModel
{
    public int PublicProperty { get; set; } = 1;
    public static int PublicStaticProperty { get; set; } = 1;

    private static int PrivateStaticProperty { get; set; } = 1;
    private int PrivateProperty { get; set; } = 2;

    public static int PublicStaticField = 1;
    public int PublicField = 1;
}

internal class Program
{
    private static void Main(string[] args)
    {
        {
            var value = TestModel.PublicStaticProperty;
            TestModel.PublicStaticProperty = 2;
        }
        {
            var value = TestModel.PublicStaticField;
            TestModel.PublicStaticField = 2;
        }

        var obj = new TestModel();
        {
            var value = obj.PublicProperty;
            obj.PublicProperty = 2;
        }
        {
            var value = obj.PublicField;
            obj.PublicField = 2;
        }
        {
            var value = obj.ILAccess().GetValue<int>("PrivateProperty");
            Console.WriteLine("PrivateProperty: " + value);
        }
        {
            var value = obj.ILAccess().GetValue<int>("PrivateStaticProperty");
            Console.WriteLine("PrivateStaticProperty: " + value);
        }

        Console.Read();
    }
}
