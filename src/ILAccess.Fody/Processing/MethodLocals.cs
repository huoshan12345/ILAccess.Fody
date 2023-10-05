using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fody;
using ILAccess.Fody.Extensions;
using ILAccess.Fody.Models;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILAccess.Fody.Processing
{
    internal class MethodLocals
    {
        private readonly Dictionary<string, VariableDefinition> _localsByName = new();
        private readonly List<VariableDefinition> _localsByIndex = new();
        private readonly MethodDefinition _method;

        public MethodLocals(MethodDefinition method)
        {
            _method = method;
            foreach (var variable in _method.Body.Variables)
            {
                _localsByIndex.Add(variable);
                _localsByName.Add(variable.ToString(), variable);
            }
        }

        public VariableDefinition AddLocalVar(LocalVarBuilder local)
        {
            var localVar = local.Build();
            _method.Body.Variables.Add(localVar);
            var name = local.Name ?? localVar.ToString();

            _method.DebugInformation.Scope?.Variables.Add(new VariableDebugInformation(localVar, name));

            if (_localsByName.ContainsKey(name))
                throw new WeavingException($"Local {local.Name} is already defined");

            _localsByName.Add(name, localVar);
            _localsByIndex.Add(localVar);

            return localVar;
        }
    }
}
