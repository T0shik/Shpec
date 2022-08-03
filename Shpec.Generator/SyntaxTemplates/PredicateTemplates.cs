namespace Shpec.Generator.SyntaxTemplates;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

internal class PredicateTemplates
{
    

    public ExpressionSyntax positive(string identifier) =>
        BinaryExpression(
                SyntaxKind.GreaterThanExpression,
                IdentifierName(Identifier(identifier)),
                LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0))
            );
}
