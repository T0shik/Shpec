using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Generators;

class DefinitionsAggregate : ISyntaxReceiver
{
    public readonly Dictionary<string, Declaration> Definitions = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return;
        }

        if (!invocationExpressionSyntax.Expression.ToString().Equals("_schema.define"))
        {
            return;
        }

        var propertyArguments = invocationExpressionSyntax.ArgumentList.Arguments.ToList();

        if (propertyArguments is not { Count: > 0 })
        {
            return;
        }

        var namespaceDeclaration = invocationExpressionSyntax.GetParent<FileScopedNamespaceDeclarationSyntax>();
        var classDeclaration = invocationExpressionSyntax.GetParent<ClassDeclarationSyntax>();
        var propertyNames = propertyArguments.Select(x => x.Expression.ToString().Split('.').Last()).ToImmutableArray();

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
