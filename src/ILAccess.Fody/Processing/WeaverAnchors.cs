namespace ILAccess.Fody.Processing;

internal class WeaverAnchors
{
    public const string AssemblyName = "ILAccess";
    public const string TypeName = "ILAccessor";
    public const string TypeGenericName = "ILAccessor`1";
    public const string MethodName = "ILAccess";
    public const string MethodCall = $"{MethodName}<T>()";

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MethodNames
    {
        public const string GetPropertyValue = "GetPropertyValue";
        public const string SetPropertyValue = "SetPropertyValue";
        public const string GetFieldValue = "GetFieldValue";
        public const string SetFieldValue = "SetFieldValue";

        public static string[] All = { GetPropertyValue, SetPropertyValue, GetFieldValue, SetFieldValue };
    }
}