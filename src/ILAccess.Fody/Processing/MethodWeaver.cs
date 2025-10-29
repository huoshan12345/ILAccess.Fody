using System.IO;
using System.Reflection;
using MoreFodyHelpers.Building;
using MoreFodyHelpers.Support;

namespace ILAccess.Fody.Processing;

internal sealed class MethodWeaver
{
    private readonly ModuleWeavingContext _context;
    private readonly MethodDefinition _method;
    private readonly MethodWeaverLogger _log;
    private readonly WeaverILProcessor _il;
    private readonly SequencePointMapper _sequencePoints;
    private readonly CustomAttribute _anchorAttribute;

    public MethodWeaver(ModuleWeavingContext context, MethodDefinition method, CustomAttribute anchorAttribute, IWeaverLogger log)
    {
        _context = context;
        _method = method;
        _anchorAttribute = anchorAttribute;
        _il = new WeaverILProcessor(method);
        _sequencePoints = new SequencePointMapper(method, true);
        _log = new MethodWeaverLogger(log, _method);
    }

    public static bool TryProcess(
        ModuleWeavingContext context,
        MethodDefinition method,
        IWeaverLogger log,
        [NotNullWhen(true)] out string? assemblyName)
    {
        assemblyName = null;

        var attr = method.CustomAttributes.FirstOrDefault(m => m.AttributeType.FullName == WeaverAnchors.AttributeName);
        if (attr == null)
            return false;

        var weaver = new MethodWeaver(context, method, attr, log);
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
            throw new InvalidOperationException(
                $"Unexpected error occured while processing method {_method.FullName}: {ex.Message}", ex);
        }
        finally
        {
            _method.CustomAttributes.RemoveWhere(m => m.AttributeType.FullName == WeaverAnchors.AttributeName);
        }
    }

    private static readonly HashSet<string> CtorNames = [".ctor", ".cctor"];

    private bool ProcessImpl([NotNullWhen(true)] out string? assemblyName)
    {
        var kind = (ILAccessorKind)_anchorAttribute.ConstructorArguments.Single().Value;
        var name = (string?)_anchorAttribute.Properties.SingleOrDefault().Argument.Value;
        if (name is null && kind != ILAccessorKind.Constructor)
            throw new WeavingException($"The property 'Name' should be specific for {kind} on {WeaverAnchors.AttributeName}");

        TypeReference? typeRef;
        if (kind == ILAccessorKind.Constructor)
        {
            typeRef = _method.ReturnType;
        }
        else
        {
            if (_method.Parameters.Count == 0)
                throw new WeavingException($"For {kind}, the type of the first argument of the annotated extern static method should be the owning type.");

            typeRef = _method.Parameters[0].ParameterType;
        }

        var type = typeRef.Resolve();

        var isReturnRef = _method.ReturnType.IsByReference;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (kind)
        {
            case ILAccessorKind.Constructor:
            case ILAccessorKind.Method:
            case ILAccessorKind.StaticMethod:
            {
                var isCtor = kind == ILAccessorKind.Constructor;
                var isCtorMethod = kind != ILAccessorKind.Constructor && name != null && CtorNames.Contains(name);
                var isStatic = kind == ILAccessorKind.StaticMethod;
                var paras = isCtor
                    ? _method.Parameters
                    : _method.Parameters.Skip(1);
                var parameterTypes = paras.Select(p => p.ParameterType).ToArray();
                var method = _context.FindMethod(type, name, parameterTypes, isCtor || isCtorMethod, isStatic);

                // do not use _context.Module.ImportReference(method); here because it won't do anything when method is from the same module.
                var importer = _context.Module.GetMetadataImporter();
                var methodRef = importer.ImportReference(method, null);

                // after importing, the DeclaringType will be an open generic type.
                // so needs to set the method declaring type to the correct instantiated type.
                methodRef.DeclaringType = typeRef;

                var start = isStatic ? 1 : 0;
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

                _il.IL.Append(_il.Create(callCode, methodRef));
                _il.IL.Append(_il.Create(OpCodes.Ret));

                assemblyName = method.Module.Assembly.Name.Name;
                return true;
            }
            case ILAccessorKind.Field:
            case ILAccessorKind.StaticField:
            {
                var isStatic = kind == ILAccessorKind.StaticField;
                var field = _context.FindField(type, name, isStatic);
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

file static class Extensions
{
    private static string TrimEnd(this string str, string value)
    {
        var index = str.LastIndexOf(value, StringComparison.Ordinal);
        return index >= 0
            ? str.Substring(0, index)
            : str;
    }

    private static readonly string DirectorySeparator = Path.DirectorySeparatorChar.ToString();

    private static bool TryGetImplementationAssemblyPath(string referenceAssemblyPath, [NotNullWhen(true)] out string? implPath)
    {
        implPath = null;
        // C:\Program Files (x86)\dotnet\packs\Microsoft.NETCore.App.Ref\9.0.10\ref\net9.0\System.Collections.Immutable.dll
        // C:\Program Files (x86)\dotnet\shared\Microsoft.NETCore.App\9.0.10\System.Collections.Immutable.dll
        const string refSuffix = ".Ref";
        var sections = referenceAssemblyPath.Split([DirectorySeparator], StringSplitOptions.None);
        var packsIndex = Array.IndexOf(sections, "packs");
        if (packsIndex < 0
            || sections.Length <= packsIndex + 3
            || sections[packsIndex + 1].EndsWith(refSuffix) == false)
            return false;

        var newSections = new List<string>(sections.Take(packsIndex))
        {
            "shared",
            sections[packsIndex + 1].TrimEnd(refSuffix), // framework name
            sections[packsIndex + 2], // version
            sections.Last() // assembly name
        };
        implPath = string.Join(DirectorySeparator, newSections.ToArray());
        return true;
    }

    private static bool TryResolveFromImplementationAssembly(this ModuleWeavingContext context, TypeReference typeRef, [NotNullWhen(true)] out TypeDefinition? typeDef)
    {
        typeDef = null;

        var assemblyName = typeRef.Module.Assembly.Name.Name;
        var path = typeRef.Module.FileName;

        if (TryGetImplementationAssemblyPath(path, out var implPath) == false
            || File.Exists(implPath) == false)
        {
            return false;
        }

        context.Module.AssemblyResolver.UpdateAssemblyResolver(typeRef.Module.Assembly.Name.Name, implPath);

        // use FromAssemblyNameAndTypeName to find type in declared and forwarded types.
        var tRef = TypeRefBuilder.FromAssemblyNameAndTypeName(context, assemblyName, typeRef.FullName).Build();
        typeDef = tRef.Resolve();
        return typeDef != null;
    }

    private static readonly Type? Type_AssemblyResolver = Type.GetType("AssemblyResolver, FodyIsolated");
    private static readonly FieldInfo? Field_AssemblyResolver_ReferenceDictionary
        = Type_AssemblyResolver?.GetField("referenceDictionary", BindingFlags.NonPublic | BindingFlags.Instance);

    private static void UpdateAssemblyResolver(this IAssemblyResolver resolver, string assemblyName, string path)
    {
        var type = resolver.GetType();
        if (type != Type_AssemblyResolver || Field_AssemblyResolver_ReferenceDictionary is not { } field)
            return;

        var referenceDictionary = (Dictionary<string, string>)field.GetValue(resolver);
        referenceDictionary[assemblyName] = path;

        // try to add System.Private.CoreLib reference if missing
        if (referenceDictionary.ContainsKey(AssemblyNames.SystemPrivateCoreLib))
            return;

        var file = new FileInfo(path);
        if (file.Directory is not { Exists: true } dir)
            return;

        var lib = Path.Combine(dir.FullName, AssemblyNames.SystemPrivateCoreLib + ".dll");
        if (File.Exists(lib) == false)
            return;

        referenceDictionary[AssemblyNames.SystemPrivateCoreLib] = lib;
    }

    public static MethodDefinition FindMethod(this ModuleWeavingContext context, TypeDefinition typeDef, string? name, IReadOnlyList<TypeReference> parameterTypes, bool isCtor, bool isStatic)
    {
        var methods = GetMethods(typeDef);
        // ReSharper disable once InvertIf
        if (methods.Length == 0)
        {
            if (context.TryResolveFromImplementationAssembly(typeDef, out var def))
            {
                methods = GetMethods(def);
            }
        }

        if (methods.Length == 1)
            return methods[0];

        var paraTypeNames = parameterTypes.Select(m => m.Name).JoinWith(", ");
        throw methods.Length switch
        {
            0 => new WeavingException($"Method '{name}({paraTypeNames})' not found on type '{typeDef.FullName}'."),
            _ => new WeavingException($"{methods.Length} methods '{name}({paraTypeNames})' found on type '{typeDef.FullName}'.")
        };

        MethodDefinition[] GetMethods(TypeDefinition def)
        {
            return def.Methods
                .Where(m => m.IsStatic == isStatic
                            && m.IsConstructor == isCtor
                            && (isCtor == false && m.Name == name || isCtor)
                            && m.Parameters.Select(x => x.ParameterType)
                                .SequenceEqual(parameterTypes, TypeReferenceEqualityComparer.Instance))
                .ToArray();
        }
    }

    public static FieldDefinition FindField(this ModuleWeavingContext context, TypeDefinition typeDef, string? name, bool isStatic)
    {
        var fields = GetFields(typeDef);
        // ReSharper disable once InvertIf
        if (fields.Length == 0)
        {
            if (context.TryResolveFromImplementationAssembly(typeDef, out var def))
            {
                fields = GetFields(def);
            }
        }

        return fields.Length switch
        {
            0 => throw new WeavingException($"{(isStatic ? "Static field" : "Field")} '{name}' not found on type '{typeDef.FullName}'."),
            > 1 => throw new WeavingException($"Multiple {(isStatic ? "static" : "")} fields named '{name}' found on type '{typeDef.FullName}'."),
            _ => fields[0],
        };


        FieldDefinition[] GetFields(TypeDefinition def)
        {
            return def.Fields
                .Where(m => m.Name == name && m.IsStatic == isStatic)
                .ToArray();
        }
    }
}