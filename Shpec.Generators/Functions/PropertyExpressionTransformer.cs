using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.Functions;

internal class PropertyExpressionTransformer
{
    private readonly TransformFactory _factory;
    private readonly List<PropertyDefinition> _propertyDefinitions;
    private readonly List<ComputedPropertyDefinition> _computedPropertyDefinitions;

    public PropertyExpressionTransformer(
        TransformFactory factory,
        List<PropertyDefinition> propertyDefinitions,
        List<ComputedPropertyDefinition> computedPropertyDefinitions
    )
    {
        _factory = factory;
        _propertyDefinitions = propertyDefinitions;
        _computedPropertyDefinitions = computedPropertyDefinitions;
    }

    public ExpressionSyntax Transform(ExpressionSyntax exp) => exp switch
    {
        BinaryExpressionSyntax a => Transform(a),
        InterpolatedStringExpressionSyntax a => Transform(a),
        ParenthesizedLambdaExpressionSyntax a => Transform(a),

        ElementAccessExpressionSyntax a => Transform(a),
        MemberAccessExpressionSyntax a => Transform(a),
        AssignmentExpressionSyntax a => TransformAssignmentExpressionSyntax(a),
        _ => exp
    };

    private ExpressionSyntax TransformAssignmentExpressionSyntax(AssignmentExpressionSyntax aes) =>
        aes.WithLeft(Transform(aes.Left))
            .WithRight(Transform(aes.Right));

    private ExpressionSyntax Transform(BinaryExpressionSyntax exp) => exp
        .WithLeft(Transform(exp.Left))
        .WithRight(Transform(exp.Right));

    private ExpressionSyntax Transform(InterpolatedStringExpressionSyntax exp)
    {
        return exp.WithContents(new(
            exp.Contents.Select(x =>
            {
                if (x is not InterpolationSyntax interpolationSyntax)
                {
                    return x;
                }

                return interpolationSyntax.WithExpression(Transform(interpolationSyntax.Expression));
            })
        ));
    }

    private ExpressionSyntax Transform(ParenthesizedLambdaExpressionSyntax exp)
    {
        var tr = _factory.StatementTransformer();
        var b = exp.Block;
        if (b == null)
        {
            throw new ArgumentNullException("block is null, don't know what this means");
        }

        return exp.WithBlock(
            b.WithStatements(
                new SyntaxList<StatementSyntax>(b.Statements.Select(tr.Transform))
            )
        );
    }

    private ExpressionSyntax Transform(ElementAccessExpressionSyntax exp) =>
        exp.WithExpression(Transform(exp.Expression));

    private ExpressionSyntax Transform(MemberAccessExpressionSyntax exp)
    {
        var property = MatchingMember();
        
        if (property == null)
        {
            if (exp.Expression is MemberAccessExpressionSyntax next)
            {
                return exp.WithExpression(Transform(next));
            }

            return exp;
        }

        return IdentifierName(property);

        string? MatchingMember()
        {
            var memberName = exp.Name.ToString();
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