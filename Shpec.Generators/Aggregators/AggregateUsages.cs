using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generators.SyntaxTemplates;
using Shpec.Generators.Utils;

namespace Shpec.Generators.Aggregators;

class AggregateUsages : ISyntaxReceiver
{
    public readonly Dictionary<string, Usage> Captures = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not PropertyDeclarationSyntax { Type: IdentifierNameSyntax { Identifier.Text: "Members" } } propertyDeclaration)
        {
            return;
        }

        var clazz = ResolveClassHierarchy(propertyDeclaration.Parent);
        var propertyNames = GetProperties(propertyDeclaration);
        var namespaceDeclaration = propertyDeclaration.GetParent<FileScopedNamespaceDeclarationSyntax>();
        var ns = namespaceDeclaration.Name.ToString();

        var key = $"{ns}.{clazz}";
        if (Captures.ContainsKey(key))
        {
            var d = Captures[key];
            Captures[key] = d with { Members = d.Members.Concat(propertyNames) };
        }
        else
        {
            Captures[key] = new(ns, clazz, propertyNames);
        }
    }

    private static ClassDeclaration ResolveClassHierarchy(SyntaxNode syntaxNode) => syntaxNode switch
    {
        ClassDeclarationSyntax a => CaptureClassHierarchy(a),
        StructDeclarationSyntax a => CaptureClassHierarchy(a, stract: true),
        RecordDeclarationSyntax { RawKind: (int)SyntaxKind.RecordDeclaration } a => CaptureClassHierarchy(a, record: true),
        RecordDeclarationSyntax { RawKind: (int)SyntaxKind.RecordStructDeclaration } a => CaptureClassHierarchy(a, record: true, stract: true),
        _ => throw new ShpecAggregationException("failed to ResolveClassHierarchy for enclosing type", syntaxNode),
    };

    private static ClassDeclaration CaptureClassHierarchy(TypeDeclarationSyntax typeDeclaration, bool record = false, bool stract = false)
    {
        var id = typeDeclaration.Identifier.ToString();

        var accessibility = typeDeclaration.Modifiers.First().ValueText switch
        {
            "public" => SyntaxKind.PublicKeyword,
            "private" => SyntaxKind.PrivateKeyword,
            "internal" => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.InternalKeyword
        };

        var statik = typeDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));

        var parent = typeDeclaration.TryGetParent<TypeDeclarationSyntax>();
        if (parent == null)
        {
            return new(id, accessibility, null, statik, record, stract);
        }

        return new(id, accessibility, ResolveClassHierarchy(parent), statik, record, stract);
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
}