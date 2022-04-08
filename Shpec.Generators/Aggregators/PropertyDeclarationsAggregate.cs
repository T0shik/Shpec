using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Aggregators;

class PropertyDeclarationsAggregate : ISyntaxReceiver
{
    public List<PropertyDefinition> Declarations { get; set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
        {
            return;
        }

        if (!objectCreationExpressionSyntax.Type.GetText().ToString().Equals("_property"))
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
        
        Declarations.Add(new PropertyDefinition(identifier, type));
    }
}