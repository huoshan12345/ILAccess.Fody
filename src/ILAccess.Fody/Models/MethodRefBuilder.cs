using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fody;
using ILAccess.Fody.Extensions;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace ILAccess.Fody.Models
{
    internal class MethodRefBuilder
    {
        private readonly MethodReference _method;

        private MethodRefBuilder(ModuleDefinition module, TypeReference typeRef, MethodReference method)
        {
            _method = module.ImportReference(module.ImportReference(method.MapToScope(typeRef.Scope, module)).MakeGeneric(typeRef));
        }

        public static MethodRefBuilder MethodByName(ModuleDefinition module, TypeReference typeRef, string methodName)
            => new(module, typeRef, FindMethod(typeRef, methodName, null, null, null));

        public static MethodRefBuilder MethodByNameAndSignature(ModuleDefinition module, TypeReference typeRef, string methodName, int? genericArity, TypeReference? returnType, IReadOnlyList<TypeReference> paramTypes)
            => new(module, typeRef, FindMethod(typeRef, methodName, genericArity, returnType, paramTypes ?? throw new ArgumentNullException(nameof(paramTypes))));

        private static MethodReference FindMethod(TypeReference typeRef, string methodName, int? genericArity, TypeReference? returnType, IReadOnlyList<TypeReference>? paramTypes)
        {
            var typeDef = typeRef.ResolveRequiredType();

            var methods = typeDef.Methods.Where(m => m.Name == methodName);

            if (genericArity != null)
            {
                methods = genericArity == 0
                    ? methods.Where(m => !m.HasGenericParameters)
                    : methods.Where(m => m.HasGenericParameters && m.GenericParameters.Count == genericArity);
            }

            if (returnType != null)
                methods = methods.Where(m => m.ReturnType.FullName == returnType.FullName);

            if (paramTypes != null)
                methods = methods.Where(m => SignatureMatches(m, paramTypes));

            var methodList = methods.ToList();

            return methodList.Count switch
            {
                1 => methodList.Single(),
                0 => throw new WeavingException($"Method {GetDisplaySignature(methodName, genericArity, returnType, paramTypes)} not found in type {typeDef.FullName}"),
                _ => throw new WeavingException($"Ambiguous method {GetDisplaySignature(methodName, genericArity, returnType, paramTypes)} in type {typeDef.FullName}")
            };
        }

        private static bool SignatureMatches(MethodReference method, IReadOnlyList<TypeReference> paramTypes)
        {
            if (method.Parameters.Count != paramTypes.Count)
                return false;

            for (var i = 0; i < paramTypes.Count; ++i)
            {
                var paramType = paramTypes[i];
                if (paramType == null)
                    return false;

                if (method.Parameters[i].ParameterType.FullName != paramType.FullName)
                    return false;
            }

            return true;
        }

        private static string GetDisplaySignature(string methodName, int? genericArity, TypeReference? returnType, IReadOnlyList<TypeReference>? paramTypes)
        {
            if (genericArity is null && returnType is null && paramTypes is null)
                return "'" + methodName + "'";

            var sb = new StringBuilder();

            if (returnType != null)
                sb.Append(returnType.FullName).Append(' ');

            sb.Append(methodName);

            switch (genericArity)
            {
                case 0:
                case null:
                    break;

                case 1:
                    sb.Append("<T>");
                    break;

                default:
                    sb.Append('<');

                    for (var i = 0; i < genericArity.GetValueOrDefault(); ++i)
                    {
                        if (i != 0)
                            sb.Append(", ");

                        sb.Append('T').Append(i);
                    }

                    sb.Append('>');
                    break;
            }

            if (paramTypes != null)
            {
                sb.Append('(');

                for (var i = 0; i < paramTypes.Count; ++i)
                {
                    if (i != 0)
                        sb.Append(", ");

                    sb.Append(paramTypes[i].FullName);
                }

                sb.Append(')');
            }

            return sb.ToString();
        }

        public static MethodRefBuilder PropertyGet(ModuleDefinition module, TypeReference typeRef, string propertyName)
        {
            var property = FindProperty(typeRef, propertyName);

            if (property.GetMethod == null)
                throw new WeavingException($"Property '{propertyName}' in type {typeRef.FullName} has no getter");

            return new MethodRefBuilder(module, typeRef, property.GetMethod);
        }

        private static PropertyDefinition FindProperty(TypeReference typeRef, string propertyName)
        {
            var typeDef = typeRef.ResolveRequiredType();

            var properties = typeDef.Properties.Where(p => p.Name == propertyName).ToList();

            return properties.Count switch
            {
                1 => properties.Single(),
                0 => throw new WeavingException($"Property '{propertyName}' not found in type {typeDef.FullName}"),
                _ => throw new WeavingException($"Ambiguous property '{propertyName}' in type {typeDef.FullName}")
            };
        }

        public MethodReference Build()
            => _method;

        public override string ToString() => _method.ToString();
    }
}
