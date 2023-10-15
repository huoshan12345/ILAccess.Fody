using System;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable CA2211
#pragma warning disable IDE0051

namespace ILAccess.Example;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public class TestModel
{
    public int PublicProperty { get; set; } = 1;
    public static int PublicStaticProperty { get; set; } = 1;

    private static int PrivateStaticProperty { get; set; } = 1;
    private int PrivateProperty { get; set; } = 1;

    public static int PublicStaticField = 1;
    public int PublicField = 1;
}

internal class Program
{
    private static void Main(string[] args)
    {
        var obj = new TestModel();

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
