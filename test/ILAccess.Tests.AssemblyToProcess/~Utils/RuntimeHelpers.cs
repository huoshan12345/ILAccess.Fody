using System.Runtime.Serialization;

#pragma warning disable SYSLIB0050 // Formatter-based serialization is obsolete

namespace ILAccess.Tests.AssemblyToProcess;

public static class RuntimeHelpers
{
    public static T GetUninitializedObject<T>()
    {
        return (T)FormatterServices.GetUninitializedObject(typeof(T));
    }
}
