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

        var emitted = false;
        foreach (var type in ModuleDefinition.GetTypes())
        {
            foreach (var method in type.Methods)
            {
                _log.Debug($"Processing: {method.FullName}");

                try
                {
                    emitted = new MethodWeaver(context, ModuleDefinition, method, _log).Process() || emitted;
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

    public override IEnumerable<string> GetAssembliesForScanning() => [];

    protected virtual void AddError(string message, SequencePoint? sequencePoint)
        => _log.Error(message, sequencePoint);
}
