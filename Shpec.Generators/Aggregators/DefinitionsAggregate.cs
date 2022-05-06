using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Aggregators;

class DefinitionsAggregate : ISyntaxReceiver
{
    public readonly Dictionary<string, Declaration> Definitions = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not PropertyDeclarationSyntax { Type: IdentifierNameSyntax { Identifier.Text: "Properties" } } propertyDeclaration)
        {
            return;
        }

        var propertyNames = GetProperties(propertyDeclaration);

        var namespaceDeclaration = propertyDeclaration.GetParent<FileScopedNamespaceDeclarationSyntax>();
        var classDeclaration = propertyDeclaration.GetParent<ClassDeclarationSyntax>();
        var ns = namespaceDeclaration.Name.ToString();
        var clazz = CaptureClassHierarchy(classDeclaration);

        var key = $"{ns}.{clazz}";
        if (Definitions.ContainsKey(key))
        {
            var d = Definitions[key];
            Definitions[key] = d with { Members = d.Members.Concat(propertyNames) };
        }
        else
        {
            Definitions[key] = new(ns, clazz, propertyNames);
        }
    }

    private ImmutableArray<string> GetProperties(PropertyDeclarationSyntax propertyDeclaration)
    {
        if (propertyDeclaration.ExpressionBody == null)
        {
            throw new($"No expression body for properties in {propertyDeclaration.FullSpan}");
        }

        if (propertyDeclaration.ExpressionBody.Expression is ImplicitObjectCreationExpressionSyntax a)
        {
            return a.ArgumentList.Arguments
                .Select(x => x.Expression.ToString().Split('.').Last())
                .ToImmutableArray();
        }


        throw new($"Unknown Scenario {propertyDeclaration.FullSpan}");
    }

    private static ClassDeclaration CaptureClassHierarchy(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var id = classDeclarationSyntax.Identifier.ToString();

        var accessibility = classDeclarationSyntax.Modifiers.First().ValueText switch
        {
            "public" => SyntaxKind.PublicKeyword,
            "private" => SyntaxKind.PrivateKeyword,
            "internal" => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.InternalKeyword
        };

        var statik = classDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));

        var parent = classDeclarationSyntax.TryGetParent<ClassDeclarationSyntax>();
        return parent == null
            ? new(id, accessibility, null, statik)
            : new(id, accessibility, CaptureClassHierarchy(parent), statik);
    }
}