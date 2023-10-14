using FieldAttributes = Mono.Cecil.FieldAttributes;
using PropertyAttributes = Mono.Cecil.PropertyAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace ILAccess.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    private readonly Logger _log;

    public ModuleWeaver()
    {
        _log = new Logger(this);
        _stringType = ModuleDefinition.ImportReference(typeof(string));
    }

    private readonly TypeReference _stringType;

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
            var attr = GetOrAddIgnoresAccessChecksToAttribute();
            var ctors = attr.GetConstructors();
            var ctor = ctors
                .Where(m => m.Parameters.Count == 1)
                .FirstOrDefault(m => m.Parameters[0].ParameterType.IsEqualTo(_stringType));

            var attribute = new CustomAttribute(ctor);
            var arg = new CustomAttributeArgument(_stringType, ModuleDefinition.Assembly.Name.Name);
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

        var attrType = ModuleDefinition.ImportReference(typeof(Attribute));
        var typeDef = new TypeDefinition(ns, name, TypeAttributes.Class | TypeAttributes.NotPublic, attrType);
        var property = new PropertyDefinition("AssemblyName", PropertyAttributes.None, _stringType);
        var fieldName = "<" + property.Name + ">k__BackingField";
        var field = new FieldDefinition(fieldName, FieldAttributes.CompilerControlled, _stringType);

        return typeDef;
    }

    public override IEnumerable<string> GetAssembliesForScanning() => Enumerable.Empty<string>();

    protected virtual void AddError(string message, SequencePoint? sequencePoint)
        => _log.Error(message, sequencePoint);
}
