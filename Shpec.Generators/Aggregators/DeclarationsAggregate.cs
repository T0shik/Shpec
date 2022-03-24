using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Generators;

class DeclarationsAggregate : ISyntaxReceiver
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

        var fieldDeclaration = attributeSyntax.GetParent<FieldDeclarationSyntax>(throwError: true);

        var propertyArguments = fieldDeclaration.DescendantNodes(_ => true)
            .OfType<ArgumentSyntax>()
            .ToList();

        if (propertyArguments is not { Count: > 0 })
        {
            return;
        }

        var namespaceDeclaration = fieldDeclaration.GetParent<FileScopedNamespaceDeclarationSyntax>(throwError: true);
        var classDeclaration = fieldDeclaration.GetParent<ClassDeclarationSyntax>(throwError: true);
        var propertyNames = propertyArguments.Select(x => x.Expression.ToString().Split('.').Last()).ToImmutableArray();

        var ns = namespaceDeclaration.Name.ToString();
        var clazz = CaptureClassHierarchy(classDeclaration);

        var key = $"{ns}.{clazz}";
        if (Declarations.ContainsKey(key))
        {
            var d = Declarations[key];
            Declarations[key] = d with { Properties = d.Properties.Concat(propertyNames) };
        }
        else
        {
            Declarations[key] = new(ns, clazz, propertyNames);
        }
    }

    private static ClassDeclaration CaptureClassHierarchy(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var parent = classDeclarationSyntax.GetParent<ClassDeclarationSyntax>();
        var id = classDeclarationSyntax.Identifier.ToString();

        var accessibility = classDeclarationSyntax.Modifiers.First().ValueText switch
        {
            "public" => SyntaxKind.PublicKeyword,
            "private" => SyntaxKind.PrivateKeyword,
            "internal" => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.InternalKeyword
        };

        var statik = classDeclarationSyntax.Modifiers.Any(x => x.ValueText == "static");

        return parent == null
            ? new(id, accessibility, null, statik)
            : new(id, accessibility, CaptureClassHierarchy(parent), statik);
    }
}
