using System;
using System.Linq;
using System.Reflection;

namespace ILAccess.Tests.AssemblyToProcess;

public static class TypeExtensions
{
    public const BindingFlags MemberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    public static FieldInfo GetRequiredField(this Type type, string name)
    {
        return type.GetField(name, MemberBindingFlags) ?? throw new InvalidOperationException($"Cannot find field '{name}' in type '{type.FullName}'");
    }

    public static PropertyInfo GetRequiredProperty(this Type type, string name)
    {
        return type.GetProperty(name, MemberBindingFlags) ?? throw new InvalidOperationException($"Cannot find property '{name}' in type '{type.FullName}'");
    }

    public static MethodInfo GetRequiredMethod(this Type type, string name)
    {
        return type.GetMethod(name, MemberBindingFlags) ?? throw new InvalidOperationException($"Cannot find method '{name}' in type '{type.FullName}'");
    }

    public static MethodInfo GetRequiredMethod(this Type type, string name, int genericArgumentCount, params Type[] paramTypes)
    {
        return type.GetMethods()
            .Where(m => m.Name == name)
            .Select(m => (Method: m, Params: m.GetParameters(), Args: m.GetGenericArguments()))
            .Where(x => x.Args.Length == genericArgumentCount
                        && x.Params.Length == paramTypes.Length
                        && x.Params.Select(m => m.ParameterType).SequenceEqual(paramTypes))
            .Select(x => x.Method)
            .FirstOrDefault() ?? throw new InvalidOperationException($"Cannot find method '{name}<`{genericArgumentCount}>({paramTypes.Select(m => m.Name).JoinWith(", ")})' in type '{type.FullName}'");
    }
}