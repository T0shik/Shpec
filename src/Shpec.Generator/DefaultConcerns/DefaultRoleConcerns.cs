using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.DefaultConcerns;

internal static class DefaultRoleConcerns
{
    internal static ConcernSeed AssignContextConcern(TypeDeclaration roleType) =>
        new(
            SetRoleProperty(roleType),
            PointCut.AfterSet,
            FunctionType.Action
        );

    private static MethodDeclarationSyntax SetRoleProperty(TypeDeclaration roleType) =>
        MethodDeclaration(
                PredefinedType(Token(SyntaxKind.VoidKeyword)),
                Identifier("dummy_function_identifier")
            )
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList<ParameterSyntax>(
                        Parameter(Identifier("v"))
                            .WithType(IdentifierName(roleType.Identifier))
                    )
                )
            )
            .WithBody(
                Block(
                    SingletonList<StatementSyntax>(
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("v"),
                                    IdentifierName("Context")),
                                ThisExpression())))));
}