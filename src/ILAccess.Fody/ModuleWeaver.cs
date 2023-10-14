namespace ILAccess.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    private readonly Logger _log;

    public ModuleWeaver()
    {
        _log = new Logger(this);
    }

    public override void Execute()
    {
        var emitted = false;
        foreach (var type in ModuleDefinition.GetTypes())
        {
            foreach (var method in type.Methods)
            {
                _log.Debug($"Processing: {method.FullName}");

                try
                {
                    emitted = new MethodWeaver(ModuleDefinition, method, _log).Process() || emitted;
                }
                catch (WeavingException ex)
                {
                    AddError(ex.Message, ex.SequencePoint);
                }
            }
        }

        if (emitted)
        {
            var stringType = ModuleDefinition.ImportType<string>();
            var attr = GetOrAddIgnoresAccessChecksToAttribute();
            var ctor = attr.GetConstructor(stringType);
            var attribute = new CustomAttribute(ctor);
            var arg = new CustomAttributeArgument(stringType, ModuleDefinition.Assembly.Name.Name);
            attribute.ConstructorArguments.Add(arg);
            ModuleDefinition.Assembly.CustomAttributes.Add(attribute);
        }
    }

    private TypeDefinition GetOrAddIgnoresAccessChecksToAttribute()
    {
        const string ns = "System.Runtime.CompilerServices";
        const string name = "IgnoresAccessChecksToAttribute";
        var attr = ModuleDefinition.GetType(ns, name);
        if (attr != null)
            return attr;

        var type = ModuleDefinition.AddType(ns, name, TypeAttributes.Class | TypeAttributes.NotPublic, typeof(Attribute));
        var property = type.AddAutoProperty<string>("AssemblyName", setterAttributes: MethodAttributes.Private);
        var ctor = type.AddConstructor(instructions: new[]
        {
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Callvirt, property.GetMethod),
        });
        ctor.AddParameter<string>("assemblyName");
        return type;
    }

    public override IEnumerable<string> GetAssembliesForScanning() => Enumerable.Empty<string>();

    protected virtual void AddError(string message, SequencePoint? sequencePoint)
        => _log.Error(message, sequencePoint);
}
