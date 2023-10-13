namespace ILAccess.Fody.Processing;

internal class WeaverAnchors
{
    public const string AssemblyName = "ILAccess";
    public const string TypeName = "ILAccessor";
    public const string TypeFullName = $"{AssemblyName}.{TypeName}";

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MethodNames
    {
        public const string GetPropertyValue = "GetPropertyValue";
        public const string SetPropertyValue = "SetPropertyValue";
        public const string GetFieldValue = "GetFieldValue";
        public const string SetFieldValue = "SetFieldValue";
    }
}