namespace ILAccess.Fody.Extensions;

public static class TypeDefinitionExtensions
{
    public static PropertyDefinition AddAutoProperty<T>(this TypeDefinition type, string name,
        MethodAttributes getterAttributes = MethodAttributes.Public, MethodAttributes setterAttributes = MethodAttributes.Public)
    {
        var module = type.Module;
        var field = type.AddField<T>($"<{name}>k__BackingField", FieldAttributes.Private);
        field.CustomAttributes.Add(new CustomAttribute(module.GetConstructor<CompilerGeneratedAttribute>()));

        var getter = type.AddMethod<T>($"get_{name}", getterAttributes | MethodAttributes.Final | MethodAttributes.HideBySig
                                                      | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual);

        getter.AddAttribute<CompilerGeneratedAttribute>();
        {
            var il = getter.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);
        }

        var setter = type.AddMethod($"set_{name}", setterAttributes | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
        setter.AddAttribute<CompilerGeneratedAttribute>();
        {
            var il = setter.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldarg_1); // paraValue
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);
        }

        var property = new PropertyDefinition(name, PropertyAttributes.None, module.ImportType<T>())
        {
            GetMethod = getter,
            SetMethod = setter
        };

        type.Properties.Add(property);
        return property;
    }

    public static FieldDefinition AddField(this TypeDefinition type, string name, FieldAttributes attributes, TypeReference fieldType)
    {
        var field = new FieldDefinition(name, attributes, fieldType);
        type.Fields.Add(field);
        return field;
    }

    public static FieldDefinition AddField<T>(this TypeDefinition type, string name, FieldAttributes attributes)
    {
        return type.AddField(name, attributes, type.Module.ImportType<T>());
    }

    public static MethodDefinition AddMethod<T>(this TypeDefinition type, string name, MethodAttributes attributes)
    {
        return type.AddMethod(name, attributes, type.Module.ImportReference(typeof(T)));
    }

    public static MethodDefinition AddMethod(this TypeDefinition type, string name, MethodAttributes attributes)
    {
        return type.AddMethod(name, attributes, type.Module.ImportVoid());
    }

    public static MethodDefinition AddMethod(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType)
    {
        var m = new MethodDefinition(name, attributes, returnType);
        type.Methods.Add(m);

        return m;
    }

    public static MethodReference GetConstructor(this TypeDefinition type, params TypeReference[] parameters)
    {
        var ctors = type.GetConstructors();
        var ctor = ctors
            .Where(m => m.Parameters.Count == parameters.Length)
            .FirstOrDefault(m => m.Parameters.Select(x => x.ParameterType).SequenceEqual(parameters, TypeReferenceEqualityComparer.IgnoreRuntimeDiffInstance));
        return ctor ?? throw new ArgumentException($"There's no constructor({parameters.Select(m => m.SimpleName()).JoinWith(", ")}) in type {type.FullName}");
    }

    public static MethodDefinition AddConstructor(this TypeDefinition type, MethodAttributes attributes = MethodAttributes.Public, IEnumerable<Instruction>? instructions = null)
    {
        if (type.IsValueType)
        {
            throw new NotSupportedException($"Can't add a constructor to {type.FullName} because it's a value type.");
        }
        var ctor = new MethodDefinition(".ctor", attributes | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, type.Module.ImportVoid());
        var il = ctor.Body.GetILProcessor();
        il.Emit(OpCodes.Ldarg_0);
        foreach (var instruction in instructions.EmptyIfNull())
        {
            il.Append(instruction);
        }
        il.Emit(OpCodes.Ret);
        type.Methods.Add(ctor);
        return ctor;
    }
}