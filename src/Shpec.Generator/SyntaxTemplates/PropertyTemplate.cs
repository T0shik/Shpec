using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class PropertyTemplate
{
    public static IEnumerable<MemberDeclarationSyntax> Create(PropertySeed seed)
    {
        if (seed.Concerns.Count == 0)
        {
            yield return StandaloneProperty(seed);
        }
        else
        {
            foreach (var member in WithField(seed))
            {
                yield return member;
            }
        }
    }

    private static MemberDeclarationSyntax StandaloneProperty(PropertySeed seed)
    {
        var setter = seed switch
        {
            { Immutable: true } => SyntaxKind.InitAccessorDeclaration,
            _ => SyntaxKind.SetAccessorDeclaration,
        };

        return PropertyDeclaration(seed.Type, Identifier(seed.Identifier))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithAccessorList(
                AccessorList(
                    List(new[]
                    {
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(setter)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    })));
    }

    public static IEnumerable<MemberDeclarationSyntax> WithField(PropertySeed seed)
    {
        var fieldIdentifier = $"__{seed.Identifier.ToLower()}";
        return new MemberDeclarationSyntax[]
        {
            FieldDeclaration(VariableDeclaration(seed.Type)
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(fieldIdentifier)))))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword))),

            PropertyDeclaration(seed.Type, Identifier(seed.Identifier))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(
                    AccessorList(
                        List(
                            new[]
                            {
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            IdentifierName(fieldIdentifier)))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                        SyntaxKind.SetAccessorDeclaration)
                                    .WithBody(
                                        Block(
                                            IfStatement(
                                                BinaryExpression(
                                                    SyntaxKind.EqualsExpression,
                                                    IdentifierName("value"),
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(0))),
                                                Block(
                                                    SingletonList<StatementSyntax>(
                                                        ThrowStatement(
                                                            ObjectCreationExpression(
                                                                    IdentifierName("ArgumentException"))
                                                                .WithArgumentList(
                                                                    ArgumentList()))))),
                                            ExpressionStatement(
                                                AssignmentExpression(
                                                    SyntaxKind.SimpleAssignmentExpression,
                                                    IdentifierName(fieldIdentifier),
                                                    IdentifierName("value")))))
                            })))
        };
    }
}