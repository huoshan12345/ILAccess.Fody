namespace ILAccess.Fody.Extensions;

public static class TypeExtensions
{
    public static T New<T>(this Type type, params object[] args)
    {
        return (T)Activator.CreateInstance(type, args);
    }
}