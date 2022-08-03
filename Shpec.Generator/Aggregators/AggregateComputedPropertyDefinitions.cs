using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Shpec.Generator.Utils;

namespace Shpec.Generator.Aggregators;

class AggregateComputedPropertyDefinitions : ISyntaxReceiver
{
    public List<ComputedPropertyDefinition> Captures { get; set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "_computed" } } invocation)
        {
            return;
        }

        var propertyDeclarationSyntax = invocation
            .GetParent<PropertyDeclarationSyntax>();

        var identifier = propertyDeclarationSyntax.Identifier.ToString();
        if (propertyDeclarationSyntax.Type is not PredefinedTypeSyntax predefinedTypeSyntax)
        {
            throw new ShpecAggregationException("computed property is not predefined", syntaxNode);
        }
        var type = predefinedTypeSyntax.Keyword.Text;
        var argument = invocation.ArgumentList.Arguments.First();

        Captures.Add(
            new ComputedPropertyDefinition(
                identifier,
                type,
                ImmutableArray<BaseValidation>.Empty,
                argument.Expression
            )
        );
    }
    
}