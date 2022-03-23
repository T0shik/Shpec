using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Generators;

public class DeclarationsAggregate : ISyntaxReceiver
{
    public readonly Dictionary<string, Declaration> Declarations = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax attributeSyntax)
        {
            return;
        }

        if (attributeSyntax.Name.ToString() != "Schema")
        {
            return;
        }

        var fieldDeclaration = attributeSyntax.GetParent<FieldDeclarationSyntax>();

        var propertyArguments = fieldDeclaration.DescendantNodes(_ => true)
            .OfType<ArgumentSyntax>()
            .ToList();

        if (propertyArguments is not { Count: > 0 })
        {
            return;
        }

        var namespaceDeclaration = fieldDeclaration.GetParent<FileScopedNamespaceDeclarationSyntax>();
        var classDeclaration = fieldDeclaration.GetParent<ClassDeclarationSyntax>();
        var propertyNames = propertyArguments.Select(x => x.GetText().ToString()).ToImmutableArray();

        var ns = namespaceDeclaration.Name.ToString();
        var clazz = classDeclaration.Identifier.ToString();

        var key = $"{ns}.{clazz}";
        Declaration declaration = new(ns, clazz, propertyNames);
        if (Declarations.ContainsKey(key))
        {
            Declarations[key] += declaration;
        }
        else
        {
            Declarations[key] = declaration;
        }
    }
}