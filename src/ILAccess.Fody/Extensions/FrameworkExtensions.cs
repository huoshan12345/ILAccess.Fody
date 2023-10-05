using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ILAccess.Fody.Models;
using ILAccess.Fody.Support;
using Mono.Cecil;

namespace ILAccess.Fody.Extensions
{
    internal static class FrameworkExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        public static Expression Convert(this Expression e, Type type) => Expression.Convert(e, type);

        public static Expression<TDelegate> Lambda<TDelegate>(this Expression e, params ParameterExpression[] parameters) where TDelegate : Delegate
            => Expression.Lambda<TDelegate>(e, parameters);

        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (var i = list.Count - 1; i >= 0; --i)
            {
                if (predicate(list[i]))
                    list.RemoveAt(i);
            }
        }

        public static TypeReference ToTypeReference(this Type type, ModuleDefinition module)
        {
            var name = type.Assembly.GetName().Name;
            if (name == TypeHelper.CoreLibAssemblyName)
            {
                // The reference System.Private.CoreLib is added by Fody.
                // Failed to execute weaver due to a failure to load it.
                // The recommended work around is to avoid using it inside a weaver.
                name = TypeHelper.RuntimeAssemblyName;
            }
            return new TypeRefBuilder(module, name, type.FullName ?? type.Name).Build();
        }

        /// <summary>
        /// see details in https://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Stack<T> Clone<T>(this Stack<T> original)
        {
            var arr = new T[original.Count];
            original.CopyTo(arr, 0);
            Array.Reverse(arr);
            return new Stack<T>(arr);
        }
    }
}
