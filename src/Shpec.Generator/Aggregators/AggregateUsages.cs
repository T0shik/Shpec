using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Utils;

namespace Shpec.Generator.Aggregators;

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
        var propertyNames = GetMembers(propertyDeclaration);
        BaseNamespaceDeclarationSyntax? namespaceDeclaration = propertyDeclaration.TryGetParent<FileScopedNamespaceDeclarationSyntax>();

        if (namespaceDeclaration == null)
        {
            namespaceDeclaration = propertyDeclaration.TryGetParent<NamespaceDeclarationSyntax>();
        }

        if (namespaceDeclaration == null)
        {
            throw new ShpecAggregationException("failed to determine namespace", syntaxNode);
        }

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

    private ImmutableArray<string> GetMembers(PropertyDeclarationSyntax propertyDeclaration)
    {
        if (propertyDeclaration.ExpressionBody == null)
        {
            throw new ShpecAggregationException("No expression body for members, use `=>` instead of `=`.", propertyDeclaration);
        }

        BaseObjectCreationExpressionSyntax exp = propertyDeclaration.ExpressionBody.Expression switch
        {
            ObjectCreationExpressionSyntax x => x,
            ImplicitObjectCreationExpressionSyntax x => x,
            _ => throw new ShpecAggregationException("Unknown member registration scenario.", propertyDeclaration),
        };

        return exp.ArgumentList.Arguments
            .Select(x => x.Expression.ToString().Split('.').Last())
            .ToImmutableArray();
    }
}