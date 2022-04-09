using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class ComputedPropertyTemplate
{
    public static List<MemberDeclarationSyntax> Create(ComputedPropertySeed seed)
    {
        return seed.Expression switch
        {
            BinaryExpressionSyntax => ArrowExpressionMember(seed),
            InterpolatedStringExpressionSyntax => ArrowExpressionMember(seed),
            ParenthesizedLambdaExpressionSyntax => ComputedFunctionMembers(seed),
            _ => throw new ArgumentException($"Unsupported computed property type: {seed.Expression.GetType().Name}, syntax: {seed.Expression.GetText()}"),
        };
    }

    private static List<MemberDeclarationSyntax> ArrowExpressionMember(ComputedPropertySeed seed)
    {
        return new() {
            PropertyDeclaration(PredefinedType(Token(seed.Type)), Identifier(seed.Identifier))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithExpressionBody(ArrowExpressionClause(seed.Expression))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        };
    }
    private static List<MemberDeclarationSyntax> ComputedFunctionMembers(ComputedPropertySeed seed)
    {
        var exp = seed.Expression as ParenthesizedLambdaExpressionSyntax;
        var functionId = $"__{seed.Identifier}";
        return new()
        {
            PropertyDeclaration(PredefinedType(Token(seed.Type)), Identifier(seed.Identifier))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithExpressionBody(ArrowExpressionClause(InvocationExpression(IdentifierName(functionId))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

            MethodDeclaration(PredefinedType(Token(seed.Type)), Identifier(functionId))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
                .WithBody(exp.Block)
        };
    }

}