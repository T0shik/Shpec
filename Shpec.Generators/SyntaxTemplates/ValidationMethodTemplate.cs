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

    public static LocalDeclarationStatementSyntax CreateResultsDeclaration()
    {
        return LocalDeclarationStatement(
            VariableDeclaration(
                IdentifierName(
                    Identifier(
                        TriviaList(),
                        SyntaxKind.VarKeyword,
                        "var",
                        "var",
                        TriviaList())))
            .WithVariables(
                SingletonSeparatedList<VariableDeclaratorSyntax>(
                    VariableDeclarator(
                        Identifier("builder"))
                    .WithInitializer(
                        EqualsValueClause(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        GenericName(
                                            Identifier("ImmutableArray"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList<TypeSyntax>(
                                                                    IdentifierName("ValidationError")))),
                                        IdentifierName("Empty")),
                                    IdentifierName("ToBuilder"))))))));
    }

    public static IfStatementSyntax CreatePropertyValidationStatements(PropertySeed propertySeed)
    {
        var ifStatementExpression = TransformValidationExpression.From(propertySeed.Validations.First(), propertySeed);

        return IfStatement(
            ifStatementExpression,
            Block(
                SingletonList<StatementSyntax>(
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("builder"),
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
                                                                Literal("todo: Some Error")))})))))))))));
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
                                    IdentifierName("builder"),
                                    IdentifierName("ToImmutableArray"))))))));
    }

    public static class TransformValidationExpression
    {
        public static ExpressionSyntax From(ValidationSeed seed, PropertySeed property) => seed switch
        {
            AdHocValidationSeed { Expression: SimpleLambdaExpressionSyntax { ExpressionBody: BinaryExpressionSyntax be } } => Transform(be, property),

            _ => throw new Exception($"{nameof(TransformValidationExpression.Transform)}: unsupported {seed} for {property}")
        };

        private static ExpressionSyntax Transform(BinaryExpressionSyntax binaryExpression, PropertySeed property)
        {
            if (binaryExpression is { Left: IdentifierNameSyntax, Right: LiteralExpressionSyntax })
            {
                return BinaryExpression(
                            InvertArithmeticComparison(binaryExpression.OperatorToken.Kind()),
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                ThisExpression(),
                                IdentifierName(property.Identifier)),
                            binaryExpression.Right
                            );
            }

            if (binaryExpression is { Left: LiteralExpressionSyntax, Right: IdentifierNameSyntax })
            {
                return BinaryExpression(
                            InvertArithmeticComparison(binaryExpression.OperatorToken.Kind()),
                            binaryExpression.Left,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                ThisExpression(),
                                IdentifierName(property.Identifier))
                            );
            }

            throw new Exception($"{nameof(TransformValidationExpression.Transform)}: unsupported {binaryExpression} for {property}");
        }

        private static SyntaxKind InvertArithmeticComparison(SyntaxKind comparison) => comparison switch
        {
            SyntaxKind.GreaterThanToken => SyntaxKind.LessThanOrEqualExpression,
            SyntaxKind.GreaterThanEqualsToken => SyntaxKind.LessThanExpression,

            SyntaxKind.LessThanToken => SyntaxKind.GreaterThanOrEqualExpression,
            SyntaxKind.LessThanEqualsToken => SyntaxKind.GreaterThanExpression,

            SyntaxKind.EqualsEqualsToken => SyntaxKind.NotEqualsExpression,
            SyntaxKind.ExclamationEqualsToken => SyntaxKind.EqualsExpression,
            _ => throw new InvalidOperationException($"Unsupported SyntaxKind {comparison} for {nameof(InvertArithmeticComparison)}")
        };
    }
}
