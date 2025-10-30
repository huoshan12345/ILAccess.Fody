namespace ILAccess.Example;

#pragma warning disable CS0169, CS0414
public static class ExceptionAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_message")]
    public static extern ref string Message(this Exception obj);

    [ILAccessor(ILAccessorKind.Field, Name = "_stackTraceString")]
    public static extern ref string StackTraceString(this Exception obj);

    [ILAccessor(ILAccessorKind.Method, Name = "GetStackTrace")]
    public static extern string GetStackTrace(this Exception obj);

    [ILAccessor(ILAccessorKind.Method, Name = nameof(Exception.GetBaseException))]
    public static extern string GetBaseException(this Exception obj);

    [ILAccessor(ILAccessorKind.StaticMethod, Name = "IsImmutableAgileException")]
    public static extern void IsImmutableAgileException(Exception? obj, Exception exception);

    public static void Test()
    {
        var ex = new Exception("xxxxxx");

        ref var value = ref ex.Message();
        Console.WriteLine($"_message: {value}");

        ref var stackTraceString = ref ex.StackTraceString();
        stackTraceString = new StackTrace().ToString();
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        Console.WriteLine($"GetStackTrace: {ex.GetStackTrace()}");
        Console.WriteLine($"GetBaseException: {ex.GetBaseException()}");
    }
}