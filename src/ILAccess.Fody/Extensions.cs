using System.IO;
using System.Reflection;
using static System.Reflection.BindingFlags;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace ILAccess.Fody;

public static class Extensions
{
    public static PropertyInfo GetRequiredProperty(this Type type, string name)
    {
        return type.GetProperty(name, Public | NonPublic | Instance | Static)
               ?? throw new InvalidOperationException($"Cannot find property '{name}' in type '{type.FullName}'");
    }

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

    public static void RemoveReference(this ModuleWeavingContext context, string assemblyName, BaseModuleWeaver weaver)
    {
        var module = context.Module;
        var libRef = module.AssemblyReferences.FirstOrDefault(m => m?.Name == assemblyName);
        if (libRef == null)
            return;

        var importScopes = new HashSet<ImportDebugInformation>();

        foreach (var method in module.GetTypes().SelectMany(t => t.Methods))
        {
            foreach (var scope in method.DebugInformation.GetScopes())
                ProcessScope(scope);
        }

        module.AssemblyReferences.Remove(libRef);

        var copyLocalFilesToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            libRef.Name + ".dll",
            libRef.Name + ".xml",
            libRef.Name + ".pdb" // We don't ship this, but future-proof that ;)
        };

        weaver.ReferenceCopyLocalPaths.RemoveAll(i => copyLocalFilesToRemove.Contains(Path.GetFileName(i)));

        void ProcessScope(ScopeDebugInformation scope)
        {
            ProcessImportScope(scope.Import);

            if (scope.HasScopes)
            {
                foreach (var childScope in scope.Scopes)
                    ProcessScope(childScope);
            }
        }

        void ProcessImportScope(ImportDebugInformation? importScope)
        {
            if (importScope == null || !importScopes.Add(importScope))
                return;

            importScope.Targets.RemoveWhere(t => t.AssemblyReference?.Name == assemblyName || t.Type.IsWeaverReferenced(context));
            ProcessImportScope(importScope.Parent);
        }
    }


    private static readonly PropertyInfo _propertyMetadataImporter = typeof(ModuleDefinition).GetRequiredProperty("MetadataImporter");

    public static IMetadataImporter GetMetadataImporter(this ModuleDefinition module)
    {
        return (IMetadataImporter)_propertyMetadataImporter.GetValue(module);
    }
}