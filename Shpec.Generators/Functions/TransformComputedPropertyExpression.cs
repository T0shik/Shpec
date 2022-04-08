using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.Functions;

internal class TransformComputedPropertyExpression
{
    private readonly List<PropertyDefinition> _propertyDefinitions;

    public TransformComputedPropertyExpression(List<PropertyDefinition> propertyDefinitions)
    {
        _propertyDefinitions = propertyDefinitions;
    }

    public ExpressionSyntax Transform(ExpressionSyntax exp) => exp switch
    {
        BinaryExpressionSyntax a => Transform(a),
        InterpolatedStringExpressionSyntax a => Transform(a),
        MemberAccessExpressionSyntax a => Transform(a),
        _ => exp
    };

    private ExpressionSyntax Transform(BinaryExpressionSyntax exp) => exp
            .WithLeft(Transform(exp.Left))
            .WithRight(Transform(exp.Right));

    private ExpressionSyntax Transform(MemberAccessExpressionSyntax exp)
    {
        var property = exp.Kind() switch
        {
            SyntaxKind.SimpleMemberAccessExpression => _propertyDefinitions.FirstOrDefault(p => p.Identifier.ToString() == exp.Name.ToString()),
            _ => throw new NotImplementedException("TransformComputedPropertyExpression BinaryExpressionSyntax GetKnownProperty unsupported MemberAccessExpressionSyntax kind.")
        };

        if (property == null)
        {
            if (exp.Expression is MemberAccessExpressionSyntax next)
            {
                return exp.WithExpression(Transform(next));
            }

            return exp;
        }

        return IdentifierName(property.Identifier);
    }

    private ExpressionSyntax Transform(InterpolatedStringExpressionSyntax exp)
    {
        return exp.WithContents(new(
            exp.Contents.Select(x =>
            {
                if(x is not InterpolationSyntax interpolationSyntax)
                {
                    return x;
                }

                return interpolationSyntax.WithExpression(Transform(interpolationSyntax.Expression));
            })
        ));
    }
}
