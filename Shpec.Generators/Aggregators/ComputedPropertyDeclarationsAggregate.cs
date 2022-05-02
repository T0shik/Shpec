﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Shpec.Generators.Aggregators;

class ComputedPropertyDeclarationsAggregate : ISyntaxReceiver
{
    public List<ComputedPropertyDefinition> Declarations { get; set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
        {
            return;
        }

        if (!objectCreationExpressionSyntax.Type.GetText().ToString().Equals("_computed"))
        {
            return;
        }

        var propertyDeclarationSyntax = objectCreationExpressionSyntax
            .GetParent<PropertyDeclarationSyntax>();

        var identifier = propertyDeclarationSyntax.Identifier.ToString();
        if (propertyDeclarationSyntax.Type is not PredefinedTypeSyntax predefinedTypeSyntax)
        {
            throw new Exception($"Error:\n{propertyDeclarationSyntax.GetText()}");
        }
        var type = predefinedTypeSyntax.Keyword.Kind();
        var argument = objectCreationExpressionSyntax.ArgumentList.Arguments.First();

        Declarations.Add(
            new ComputedPropertyDefinition(
                identifier,
                type,
                ImmutableArray<BaseValidation>.Empty,
                argument.Expression
            )
        );
    }
}