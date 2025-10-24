using MoreFodyHelpers.Support;

namespace ILAccess.Fody.Processing;

internal sealed class MethodWeaver
{
    private readonly ModuleDefinition _module;
    private readonly ModuleWeavingContext _context;
    private readonly MethodDefinition _method;
    private readonly MethodWeaverLogger _log;
    private readonly WeaverILProcessor _il;
    private readonly SequencePointMapper _sequencePoints;
    private readonly CustomAttribute _anchorAttribute;

    public MethodWeaver(ModuleWeavingContext context, ModuleDefinition module, MethodDefinition method, CustomAttribute anchorAttribute, IWeaverLogger log)
    {
        _context = context;
        _module = module;
        _method = method;
        _anchorAttribute = anchorAttribute;
        _il = new WeaverILProcessor(method);
        _sequencePoints = new SequencePointMapper(method, true);
        _log = new MethodWeaverLogger(log, _method);
    }

    public static bool TryProcess(
        ModuleWeavingContext context,
        ModuleDefinition module,
        MethodDefinition method,
        IWeaverLogger log,
        [NotNullWhen(true)] out string? assemblyName)
    {
        var attr = method.CustomAttributes.FirstOrDefault(m => m.AttributeType.FullName == WeaverAnchors.AttributeName);
        if (attr == null)
        {
            assemblyName = null;
            return false;
        }

        var weaver = new MethodWeaver(context, module, method, attr, log);
        return weaver.Process(out assemblyName);
    }

    private bool Process([NotNullWhen(true)] out string? assemblyName)
    {
        try
        {
            _log.Info($"Processing: {_method.FullName}");
            return ProcessImpl(out assemblyName);
        }
        catch (InstructionWeavingException ex)
        {
            throw new WeavingException(_log.QualifyMessage(ex.Message, ex.Instruction))
            {
                SequencePoint = _sequencePoints.GetInputSequencePoint(ex.Instruction),
            };
        }
        catch (WeavingException ex)
        {
            throw new WeavingException(_log.QualifyMessage(ex.Message))
            {
                SequencePoint = ex.SequencePoint,
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error occured while processing method {_method.FullName}: {ex.Message}", ex);
        }
    }

    private bool ProcessImpl([NotNullWhen(true)] out string? assemblyName)
    {
        if (_method.Parameters.Count == 0)
            throw new WeavingException("The method must have at least one parameter to identify the target type.");

        var typeRef = _method.Parameters[0].ParameterType;

        if (typeRef.HasGenericParameters)
        {
            var genericType = (GenericInstanceType)typeRef;
            foreach (var parameter in typeRef.GenericParameters)
            {
                genericType.GenericArguments.Add(parameter);
            }
        }

        var type = typeRef.ResolveRequiredType(_context);
        var kind = (ILAccessorKind)_anchorAttribute.ConstructorArguments.Single().Value;
        var name = (string?)_anchorAttribute.Properties.SingleOrDefault().Argument.Value;
        if (name is null && kind != ILAccessorKind.Constructor)
            throw new WeavingException($"The property 'Name' should be specific for {kind} on {WeaverAnchors.AttributeName}");

        var isReturnRef = _method.ReturnType.IsByReference;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (kind)
        {
            case ILAccessorKind.Constructor:
            case ILAccessorKind.Method:
            case ILAccessorKind.StaticMethod:
            {
                var isCtor = kind == ILAccessorKind.Constructor;
                var isStatic = kind == ILAccessorKind.StaticMethod;
                var method = FindMethod();

                var start = isCtor || isStatic ? 1 : 0;
                for (var i = start; i < _method.Parameters.Count; i++)
                {
                    _il.IL.Append(_il.IL.Create(OpCodes.Ldarg, i));
                }

                var callCode = (isCtor, isStatic) switch
                {
                    (true, _) => OpCodes.Newobj,
                    (_, true) => OpCodes.Call,
                    _ => OpCodes.Callvirt,
                };

                _il.IL.Append(_il.Create(callCode, method));
                _il.IL.Append(_il.Create(OpCodes.Ret));

                assemblyName = method.DeclaringType.Module.Assembly.Name.Name;
                return true;

                MethodDefinition FindMethod()
                {
                    var paras = _method.Parameters.Skip(1).Select(p => p.ParameterType).ToArray();
                    var methods = type.Methods.Where(m => m.IsConstructor == isCtor
                                                                  && m.IsStatic == isStatic
                                                                  && m.Parameters.Select(x => x.ParameterType)
                                                                      .SequenceEqual(paras, TypeReferenceEqualityComparer.Instance))
                        .ToArray();

                    return methods.Length switch
                    {
                        0 => throw new WeavingException($"Method '{name}' not found on type '{type.FullName}' with specified parameters."),
                        > 1 => throw new WeavingException($"Multiple methods named '{name}' found on type '{type.FullName}' with specified parameters."),
                        _ => methods[0],
                    };
                }
            }
            case ILAccessorKind.Field:
            case ILAccessorKind.StaticField:
            {
                var fields = type.Fields.Where(f => f.Name == name).ToArray();

                switch (fields.Length)
                {
                    case 0: throw new WeavingException($"Field '{name}' not found on type '{type.FullName}'.");
                    case > 1: throw new WeavingException($"Multiple fields named '{name}' found on type '{type.FullName}'.");
                }

                var field = fields[0];
                var fieldRef = new FieldReference(field.Name, field.FieldType, typeRef);

                if (field.IsStatic)
                {
                    var code = isReturnRef
                        ? OpCodes.Ldsflda
                        : OpCodes.Ldsfld;
                    _il.IL.Append(_il.Create(code, fieldRef));
                }
                else
                {
                    var code = isReturnRef
                        ? OpCodes.Ldflda
                        : OpCodes.Ldfld;
                    _il.IL.Append(_il.Create(OpCodes.Ldarg_0));
                    _il.IL.Append(_il.Create(code, fieldRef));
                }

                _il.IL.Append(_il.Create(OpCodes.Ret));

                assemblyName = field.DeclaringType.Module.Assembly.Name.Name;
                return true;
            }
        }

        assemblyName = null;
        return false;
    }
}
