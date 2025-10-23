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
                    if (MethodWeaver.TryProcess(context, ModuleDefinition, method, _log, out var assemblyName))
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
            ModuleDefinition.AddIgnoresAccessCheck(assemblyName);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning() => [];

    protected virtual void AddError(string message, SequencePoint? sequencePoint)
        => _log.Error(message, sequencePoint);
}
