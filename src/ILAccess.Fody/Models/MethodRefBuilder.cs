namespace ILAccess.Fody.Models;

internal class MethodRefBuilder
{
    private readonly MethodReference _method;

    internal MethodRefBuilder(ModuleDefinition module, TypeReference typeRef, MethodReference method)
    {
        _method = module.ImportReference(module.ImportReference(method.MapToScope(typeRef.Scope, module)).MakeGeneric(typeRef));
    }

    public MethodReference Build()
        => _method;

    public override string ToString() => _method.ToString();
}
