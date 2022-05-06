using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Shpec.Generators.Aggregators;

class ComputedPropertyDeclarationsAggregate : ISyntaxReceiver
{
    public List<ComputedPropertyDefinition> Declarations { get; set; } = new();

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
            throw new Exception($"Error:\n{propertyDeclarationSyntax.GetText()}");
        }
        var type = predefinedTypeSyntax.Keyword.Kind();
        var argument = invocation.ArgumentList.Arguments.First();

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