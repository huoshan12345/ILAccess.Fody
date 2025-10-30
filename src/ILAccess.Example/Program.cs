namespace ILAccess.Example;


internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Testing class accessors");
        TestModelAccessors.Test();

        Console.WriteLine();
        Console.WriteLine("Testing generic class accessors");
        TestModelAccessors.TestGeneric();

        Console.WriteLine();
        Console.WriteLine("Testing struct accessors");
        TestStructAccessors.Test();

        Console.WriteLine();
        Console.WriteLine("Testing Exception accessors");
        ExceptionAccessors.Test();

        Console.WriteLine();
        Console.WriteLine("Testing List<T> accessors");
        ListAccessors.Test();

        Console.WriteLine();
        Console.WriteLine("Testing List<T> accessors<T>");
        ListAccessors.TestGeneric();

        Console.Read();
    }
}