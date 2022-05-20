using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generators.Utils;

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

        ClassDeclaration? clazz = null;
        TypeDeclarationSyntax? typeDeclaration = propertyDeclaration.Parent as ClassDeclarationSyntax;
        if (typeDeclaration != null)
        {
            clazz = CaptureClassHierarchy(typeDeclaration);
        }

        if (clazz == null)
        {
            typeDeclaration = propertyDeclaration.Parent as StructDeclarationSyntax;
            if (typeDeclaration != null)
            {
                clazz = CaptureClassHierarchy(typeDeclaration, stract: true);
            }
        }

        if (clazz == null)
        {
            typeDeclaration = propertyDeclaration.Parent as RecordDeclarationSyntax;
            if (typeDeclaration != null)
            {
                clazz = CaptureClassHierarchy(typeDeclaration, record: true);
            }
        }

        if (clazz == null)
        {
            throw new ShpecAggregationException("failed to find enclosing type", syntaxNode);
        }

        var propertyNames = GetProperties(propertyDeclaration);
        var namespaceDeclaration = propertyDeclaration.GetParent<FileScopedNamespaceDeclarationSyntax>();
        var ns = namespaceDeclaration.Name.ToString();

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

        TypeDeclarationSyntax? parent = typeDeclaration.TryGetParent<ClassDeclarationSyntax>();
        if (parent != null)
        {
            if (id == "Brr")
            {
                throw new ShpecAggregationException("", typeDeclaration);
            }

            return new(id, accessibility, CaptureClassHierarchy(parent), statik, record, stract);
        }

        parent = typeDeclaration.TryGetParent<StructDeclarationSyntax>();
        if (parent != null)
        {
            return new(id, accessibility, CaptureClassHierarchy(parent, stract: true), statik, record, stract);
        }

        parent = typeDeclaration.TryGetParent<RecordDeclarationSyntax>();
        if (parent != null)
        {
            return new(id, accessibility, CaptureClassHierarchy(parent, record: true), statik, record, stract);
        }

        return new(id, accessibility, null, statik, record, stract);
    }
}