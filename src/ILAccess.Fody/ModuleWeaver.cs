using MoreFodyHelpers.Support;

namespace ILAccess.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    private readonly IWeaverLogger _log;

    public ModuleWeaver()
    {
        _log = new WeaverLogger(this);
    }

    public override void Execute()
    {
        using var context = new ModuleWeavingContext(ModuleDefinition, WeaverAnchors.AssemblyName, ProjectDirectoryPath);

        var emittedAssemblyNames = new HashSet<string>();
        foreach (var type in ModuleDefinition.GetTypes())
        {
            foreach (var method in type.Methods)
            {
                try
                {
                    if (MethodWeaver.TryProcess(context, method, _log, out var assemblyName))
                    {
                        emittedAssemblyNames.Add(assemblyName);
                    }
                }
                catch (WeavingException ex)
                {
                    AddError(ex.Message, ex.SequencePoint);
                    break;
                }
                catch (Exception ex)
                {
                    AddError(ex.Message, method.GetSequencePoint());
                    break;
                }
            }
        }

        foreach (var assemblyName in emittedAssemblyNames)
        {
            context.AddIgnoresAccessCheck(assemblyName);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning() => [];

    protected virtual void AddError(string message, SequencePoint? sequencePoint)
        => _log.Error(message, sequencePoint);
}

file static class Extensions
{
    public static TypeDefinition GetOrAddIgnoresAccessChecksToAttribute(this ModuleWeavingContext context)
    {
        var module = context.Module;
        const string ns = "System.Runtime.CompilerServices";
        const string name = "IgnoresAccessChecksToAttribute";
        var attr = module.GetType(ns, name);
        if (attr != null)
            return attr;

        var attrRef = context.ImportReference<Attribute>();
        var attrDef = attrRef.Resolve();
        var type = module.AddType(ns, name, TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.BeforeFieldInit, attrRef);
        var property = type.AddAutoProperty<string>("AssemblyName", setterAttributes: MethodAttributes.Private);
        var baseCtor = attrDef.GetConstructor();
        var ctor = type.AddConstructor(instructions:
        [
            Instruction.Create(OpCodes.Call, module.ImportReference(baseCtor)),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Callvirt, property.SetMethod)
        ]);
        ctor.AddParameter<string>("assemblyName");
        return type;
    }

    public static void AddIgnoresAccessCheck(this ModuleWeavingContext context, string? assemblyName = null)
    {
        var attr = context.GetOrAddIgnoresAccessChecksToAttribute();
        var stringType = context.ImportReference<string>();
        var ctor = attr.GetConstructor(stringType);
        var attribute = new CustomAttribute(ctor);
        var arg = new CustomAttributeArgument(stringType, assemblyName ?? attr.Module.Assembly.Name.Name);
        attribute.ConstructorArguments.Add(arg);
        attr.Module.Assembly.CustomAttributes.Add(attribute);
    }
}
