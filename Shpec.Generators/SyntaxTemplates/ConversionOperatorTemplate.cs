using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;


static class ConversionOperatorTemplate
{
    private const string FromVariableName = "__from";

    private static string GetIdentifier(NamespaceSeed seed) => $"{seed.Identifier}.{GetClassIdentifier(seed.Clazz)}";

    private static string GetClassIdentifier(ClassSeed seed) =>
        seed.Parent == null
            ? seed.Identifier
            : $"{GetClassIdentifier(seed.Parent)}.{seed.Identifier}";


    public static ConversionOperatorDeclarationSyntax Create(ConversionSeed seed)
    {
        var toId = GetIdentifier(seed.Target);
        var fromId = GetIdentifier(seed.From);
        var properties = seed.Properties;

        var assignments = new List<SyntaxNodeOrToken>();

        foreach (var identifier in properties)
        {
            assignments.Add(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(identifier),
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(FromVariableName),
                        IdentifierName(identifier)))
            );
            assignments.Add(
                Token(SyntaxKind.CommaToken)
            );
        }

        return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), IdentifierName(toId))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier(FromVariableName)).WithType(IdentifierName(fromId)))))
            .WithExpressionBody(
                ArrowExpressionClause(
                    ObjectCreationExpression(IdentifierName(toId))
                        .WithArgumentList(ArgumentList())
                        .WithInitializer(
                            InitializerExpression(
                                SyntaxKind.ObjectInitializerExpression,
                                SeparatedList<ExpressionSyntax>(assignments)))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}