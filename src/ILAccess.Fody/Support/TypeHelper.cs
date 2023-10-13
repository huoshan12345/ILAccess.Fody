using System.Linq.Expressions;
using ILAccess.Fody.Extensions;

namespace ILAccess.Fody.Support;

internal static class TypeHelper
{
    public static MethodInfo GetMethod(Assembly assembly, string typeName, string methodName, bool isStatic, bool isPublic)
    {
        var type = assembly.GetType(typeName, true) ?? throw new TypeAccessException($"Cannot find type assembly {assembly.FullName} in by name " + typeName);
        var flag = isStatic ? BindingFlags.Static : BindingFlags.Instance;
        flag |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
        var methods = type.GetMethods(flag).Where(m => m.Name == methodName).ToList();

        return methods.Count switch
        {
            0 => throw new MissingMethodException(type.FullName, methodName),
            > 1 => throw new AmbiguousMatchException($"Found more than one method in type {typeName} by name " + methodName),
            _ => methods[0]
        };
    }

    public const string CoreLibAssemblyName = "System.Private.CoreLib";
    public const string RuntimeAssemblyName = "System.Runtime";


    public static Func<TypeReference, TypeReference, bool> TypeRefEqualFunc { get; } = CreateTypeRefEqualFunc();

    private static Func<TypeReference, TypeReference, bool> CreateTypeRefEqualFunc()
    {
        var assembly = typeof(TypeReference).Assembly;
        var method = GetMethod(assembly, "Mono.Cecil.TypeReferenceEqualityComparer", "AreEqual", true, true);
        var paras = new[]
        {
            Expression.Parameter(typeof(TypeReference)),
            Expression.Parameter(typeof(TypeReference)),
        };
        var args = paras.Append(Expression.Constant(0).Convert(assembly.GetType("Mono.Cecil.TypeComparisonMode")));
        var call = Expression.Call(method, args);
        return call.Lambda<Func<TypeReference, TypeReference, bool>>(paras).Compile();
    }

    public static Func<MethodReference, MethodReference, bool> MethodRefEqualFunc { get; } = CreateMethodRefEqualFunc();

    private static Func<MethodReference, MethodReference, bool> CreateMethodRefEqualFunc()
    {
        var assembly = typeof(TypeReference).Assembly;
        var method = GetMethod(assembly, "Mono.Cecil.MethodReferenceComparer", "AreEqual", true, true);
        var paras = new[]
        {
            Expression.Parameter(typeof(MethodReference)),
            Expression.Parameter(typeof(MethodReference)),
        };
        var call = Expression.Call(method, paras.AsEnumerable());
        return call.Lambda<Func<MethodReference, MethodReference, bool>>(paras).Compile();
    }
}
