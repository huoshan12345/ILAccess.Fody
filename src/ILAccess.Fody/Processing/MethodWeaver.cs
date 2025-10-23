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

    public static bool TryProcess(ModuleWeavingContext context, ModuleDefinition module, MethodDefinition method, IWeaverLogger log)
    {
        var attr = method.CustomAttributes.FirstOrDefault(m => m.AttributeType.FullName == WeaverAnchors.AttributeName);
        if (attr == null)
            return false;

        var weaver = new MethodWeaver(context, module, method, attr, log);
        return weaver.Process();
    }

    private bool Process()
    {
        try
        {
            _log.Debug($"Processing: {_method.FullName}");
            return ProcessImpl();
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

    private bool ProcessImpl()
    {
        if (_method.Parameters.Count == 0)
            throw new WeavingException("The method must have at least one parameter to identify the target type.");

        var type = _method.Parameters[0].ParameterType.Resolve();
        var kind = (ILAccessorKind)_anchorAttribute.ConstructorArguments.Single().Value;
        var name = (string)_anchorAttribute.Properties.Single().Argument.Value;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (kind)
        {
            case ILAccessorKind.Constructor:
            {
                return true;
            }
            case ILAccessorKind.Method:
            case ILAccessorKind.StaticMethod:
            {
                return true;
            }
            case ILAccessorKind.Field:
            case ILAccessorKind.StaticField:
            {
                var isReturnRef = _method.ReturnType.IsByReference;
                var fields = type.Fields.Where(f => f.Name == name).ToArray();
                if (fields.Length == 0)
                    throw new WeavingException($"Field '{name}' not found on type '{type.FullName}'.");

                if (fields.Length > 1)
                    throw new WeavingException($"Multiple fields named '{name}' found on type '{type.FullName}'.");

                var field = fields[0];

                if (field.IsStatic)
                {
                    var code = isReturnRef
                        ? OpCodes.Ldsflda
                        : OpCodes.Ldsfld;
                    _il.IL.Append(_il.Create(code, field));
                }
                else
                {
                    var code = isReturnRef
                        ? OpCodes.Ldflda
                        : OpCodes.Ldfld;
                    _il.IL.Append(_il.Create(OpCodes.Ldarg_0));
                    _il.IL.Append(_il.Create(code, field));
                }

                _il.IL.Append(_il.Create(OpCodes.Ret));

                return true;
            }
        }

        return false;
    }
}
