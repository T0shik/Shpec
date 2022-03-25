using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Generators;

class ComputedPropertyDefinitionsAggregate : ISyntaxReceiver
{
    public List<ComputedPropertyDefinition> Definitions { get; set; } = new();

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
        if(propertyDeclarationSyntax.Type is not PredefinedTypeSyntax predefinedTypeSyntax)
        {
            throw new Exception($"Error:\n{propertyDeclarationSyntax.GetText()}");
        }
        var type = predefinedTypeSyntax.Keyword.Kind();
        var argument = objectCreationExpressionSyntax.ArgumentList.Arguments.First();

        Definitions.Add(new ComputedPropertyDefinition(identifier, type, argument.Expression));
    }
}