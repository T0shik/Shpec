using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.Functions;

internal class TransformExpression
{
    private readonly TransformFactory _factory;
    private readonly List<PropertyDefinition> _propertyDefinitions;

    public TransformExpression(TransformFactory factory, List<PropertyDefinition> propertyDefinitions)
    {
        _factory = factory;
        _propertyDefinitions = propertyDefinitions;
    }

    public ExpressionSyntax Transform(ExpressionSyntax exp) => exp switch
    {
        BinaryExpressionSyntax a => Transform(a),
        InterpolatedStringExpressionSyntax a => Transform(a),
        ParenthesizedLambdaExpressionSyntax a => Transform(a),

        ElementAccessExpressionSyntax a => Transform(a),
        MemberAccessExpressionSyntax a => Transform(a),
        _ => exp
    };

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
        var tr = _factory.TransformStatements();
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

    // todo: tranform expressions inside Bracketed Argument List 
    private ExpressionSyntax Transform(ElementAccessExpressionSyntax exp) =>
        exp.WithExpression(Transform(exp.Expression));

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
}
