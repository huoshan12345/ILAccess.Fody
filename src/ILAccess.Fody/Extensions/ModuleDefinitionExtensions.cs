namespace ILAccess.Fody.Extensions;

public static class ModuleDefinitionExtensions
{
    ///// <summary>Changes types referencing mscorlib so they appear to be defined in System.Runtime.dll</summary>
    ///// <param name="self">type to be checked</param>
    ///// <param name="mainModule">module which assembly references will be added to/removed from</param>
    ///// <returns>the same type reference passed as the parameter. This allows the method to be used in chains of calls.</returns>
    //public static TypeReference Fix(TypeReference self, ModuleDefinition mainModule)
    //{
    //    if (self.DeclaringType != null)
    //    {
    //        Fix(self.DeclaringType, mainModule);
    //    }
    //    else
    //    {
    //        if (self.Scope.Name == "System.Private.CoreLib")
    //        {
    //            if (!mainModule.AssemblyReferences.Any(a => a.Name == _systemRuntimeRef.Name))
    //            {
    //                mainModule.AssemblyReferences.Add(_systemRuntimeRef);
    //                mainModule.AssemblyReferences.Remove((AssemblyNameReference)self.Scope);
    //            }

    //            self.Scope = _systemRuntimeRef;
    //        }
    //    }

    //    return self;
    //}

    public static TypeReference ImportType(this ModuleDefinition module, Type type)
    {
        return module.ImportReference(type);
    }

    public static TypeReference ImportType<T>(this ModuleDefinition module)
    {
        return ImportType(module, typeof(T));
    }

    public static TypeReference ImportVoid(this ModuleDefinition module)
    {
        return ImportType(module, typeof(void));
    }

    public static MethodReference GetConstructor<T>(this ModuleDefinition module, params Type[] parameters)
    {
        return module.GetConstructor(typeof(T), parameters);
    }

    public static MethodReference GetConstructor(this ModuleDefinition module, Type type, params Type[] parameters)
    {
        var result = module.ImportReference(type.GetConstructor(parameters));
        return result ?? throw new ArgumentException($"There's no constructor with those parameters in type {type.FullName}");
    }

    public static TypeDefinition AddType(this ModuleDefinition module, string @namespace, string name, TypeAttributes attributes, TypeReference? baseType = null)
    {
        var typeDef = new TypeDefinition(@namespace, name, attributes, baseType);
        module.Types.Add(typeDef);
        return typeDef;
    }

    public static TypeDefinition AddType(this ModuleDefinition module, string @namespace, string name, TypeAttributes attributes, Type? baseType = null)
    {
        return module.AddType(@namespace, name, attributes, baseType == null ? null : ImportType(module, baseType));
    }
}