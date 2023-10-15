using System.Reflection;

namespace ILAccess.Fody.Extensions;

public static class AssemblyExtensions
{
    public static Type GetRequiredType(this Assembly assembly, string name, bool ignoreCase = false)
    {
        return assembly.GetType(name, true, ignoreCase) ?? throw new InvalidOperationException($"Cannot find type '{name}' in assembly '{assembly.FullName}'");
    }
}