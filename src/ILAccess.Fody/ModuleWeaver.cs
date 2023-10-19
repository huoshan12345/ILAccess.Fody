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
        var emitted = false;
        foreach (var type in ModuleDefinition.GetTypes())
        {
            foreach (var method in type.Methods)
            {
                if (ModuleDefinition.IsAssemblyReferenced(method, WeaverAnchors.AssemblyName) == false)
                    continue;

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
            ModuleDefinition.AddIgnoresAccessCheck();
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning() => Enumerable.Empty<string>();

    protected virtual void AddError(string message, SequencePoint? sequencePoint)
        => _log.Error(message, sequencePoint);
}
