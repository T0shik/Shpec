using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

internal class ValidationMethodTemplate
{
    private const string ValidationAggregateName = "__validationResults";

    public static MemberDeclarationSyntax? Create(ClassSeed seed)
    {
        List<PropertySeed> properties = new();

        foreach (var m in seed.Members)
        {
            if (m is PropertySeed { Validations: { Count: > 0 } } ps)
            {
                properties.Add(ps);
            }
        }


        if (properties.Count == 0)
        {
            return null;
        }

        return Create(properties.AsReadOnly());
    }

    private static MemberDeclarationSyntax Create(IReadOnlyCollection<PropertySeed> seeds)
    {
        List<StatementSyntax> statements = new();

        statements.Add(CreateResultsDeclaration());

        statements.AddRange(seeds.Select(CreatePropertyValidationStatements));

        statements.Add(CreateReturnStatement());

        return MethodDeclaration(IdentifierName("ValidationResult"), Identifier("Valid"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(Block(statements));
    }

    public static LocalDeclarationStatementSyntax CreateResultsDeclaration()
    {
        var validationAggregateInitializer = ObjectCreationExpression(
            GenericName(Identifier("List"))
                .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            IdentifierName("ValidationError")
                        ))))
                .WithArgumentList(ArgumentList());

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
                                Identifier(ValidationAggregateName))
                            .WithInitializer(
                                EqualsValueClause(validationAggregateInitializer)))));
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
                                    IdentifierName(ValidationAggregateName),
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
                                                            new SyntaxNodeOrToken[]
                                                            {
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
                                                                        Literal("todo: Some Error")))
                                                            })))))))))));
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
                                        IdentifierName(ValidationAggregateName),
                                        IdentifierName("AsReadOnly"))))))));
    }

    public static class TransformValidationExpression
    {
        public static ExpressionSyntax From(ValidationSeed seed, PropertySeed property) =>
            seed switch
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

        private static SyntaxKind InvertArithmeticComparison(SyntaxKind comparison) =>
            comparison switch
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