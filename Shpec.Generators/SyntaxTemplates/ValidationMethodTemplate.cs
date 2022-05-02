using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

internal class ValidationMethodTemplate
{
    public static MemberDeclarationSyntax Create(IReadOnlyCollection<PropertySeed> seeds)
    {
        List<StatementSyntax> statements = new();

        statements.Add(CreateResultsDeclaration());

        statements.AddRange(seeds.Select(x => CreatePropertyValidationStatements(x)));

        statements.Add(CreateReturnStatement());

        return MethodDeclaration(IdentifierName("ValidationResult"), Identifier("Valid"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithBody(Block(statements));
    }

    public static IfStatementSyntax CreatePropertyValidationStatements(PropertySeed propertySeed)
    {
        return IfStatement(
            BinaryExpression(
                SyntaxKind.GreaterThanExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(propertySeed.Identifier)),
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(0))),
            Block(
                SingletonList<StatementSyntax>(
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("errors"),
                                IdentifierName("Add")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        ObjectCreationExpression(
                                            IdentifierName("ValidationError"))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Argument(
                                                            LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression, 
                                                                Literal(propertySeed.Identifier))),
                                                        Token(SyntaxKind.CommaToken),
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                ThisExpression(),
                                                                IdentifierName(propertySeed.Identifier))),
                                                        Token(SyntaxKind.CommaToken),
                                                        Argument(
                                                            LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                Literal("todo: Some Error")))}))))))))
                    .WithSemicolonToken(
                        MissingToken(SyntaxKind.SemicolonToken)))));
    }

    public static LocalDeclarationStatementSyntax CreateResultsDeclaration()
    {
        return LocalDeclarationStatement(
            VariableDeclaration(
                GenericName(
                    Identifier("List"))
                .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            IdentifierName("ValidationError")))))
            .WithVariables(
                SingletonSeparatedList<VariableDeclaratorSyntax>(
                    VariableDeclarator(
                        Identifier("errors"))
                    .WithInitializer(
                        EqualsValueClause(
                            ImplicitObjectCreationExpression())))));
    }

    public static ReturnStatementSyntax CreateReturnStatement()
    {
        return ReturnStatement(
            ImplicitObjectCreationExpression()
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList<ArgumentSyntax>(
                        Argument(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("errors"),
                                    IdentifierName("AsReadOnly"))))))));
    }
}
