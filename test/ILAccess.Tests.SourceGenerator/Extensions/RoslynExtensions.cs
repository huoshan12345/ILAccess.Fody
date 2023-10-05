using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ILAccess.Tests.SourceGenerator.Extensions
{
    internal static class RoslynExtensions
    {
        public static string? GetNamespace(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
        }
    }
}
