using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.Functions;

internal class MemberAccessRewriter : CSharpSyntaxRewriter
{
    private readonly List<PropertyDefinition> _propertyDefinitions;
    private readonly List<ComputedPropertyDefinition> _computedPropertyDefinitions;

    internal MemberAccessRewriter(
        List<PropertyDefinition> propertyDefinitions,
        List<ComputedPropertyDefinition> computedPropertyDefinitions
    )
    {
        _propertyDefinitions = propertyDefinitions;
        _computedPropertyDefinitions = computedPropertyDefinitions;
    }

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        if (node.Expression is not NameSyntax)
        {
            return node;
        }

        var member = MatchingMember();
        if (member == null)
        {
            return node;
        }

        return node
            .WithExpression(ThisExpression())
            .WithName(IdentifierName(member));

        string? MatchingMember()
        {
            var memberName = node.Name.ToString();
            var match = _propertyDefinitions.FirstOrDefault(p => p.Identifier == memberName)?.Identifier;

            if (match != null)
            {
                return match;
            }

            match = _computedPropertyDefinitions.FirstOrDefault(p => p.Identifier == memberName)?.Identifier;
            return match;
        }
    }
}