namespace ILAccess.Fody.Processing;

internal class WeaverAnchors
{
    public const string AssemblyName = "ILAccess";
    public const string TypeName = "IILAccessor";
    public const string TypeGenericName = "IILAccessor`1";
    public const string MethodName = "ILAccess";
    public const string MethodCall = $"{MethodName}<T>()";

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MethodNames
    {
        public const string GetValue = "GetValue";
        public const string SetValue = "SetPropertyValue";

        public static string[] All = { GetValue, SetValue };
    }
}