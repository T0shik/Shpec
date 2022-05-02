using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.SyntaxTemplates;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

static class ConversionOperatorTemplate
{
    private const string FromVariableName = "__from";
    private const string ToVariableName = "__to";

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

        var blockStatements = new List<StatementSyntax>()
        {
            LocalDeclarationStatement(
                VariableDeclaration(IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList())))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier(ToVariableName))
                                .WithInitializer(EqualsValueClause(ObjectCreationExpression(IdentifierName(toId))
                                    .WithArgumentList(ArgumentList()))))))
        };

        foreach (var identifier in properties)
        {
            blockStatements.Add(CreateAssignmentExpression(identifier));
        }

        blockStatements.Add(ReturnStatement(IdentifierName(ToVariableName)));

        return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), IdentifierName(toId))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier(FromVariableName)).WithType(IdentifierName(fromId)))))
            .WithBody(Block(blockStatements.ToArray()));
    }

    private static ExpressionStatementSyntax CreateAssignmentExpression(string propertyIdentifier)
    {
        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(ToVariableName),
                    IdentifierName(propertyIdentifier)),
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(FromVariableName),
                    IdentifierName(propertyIdentifier))));
    }
}