using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.DefaultConcerns;

internal static class DefaultRoleConcerns
{
    internal static ConcernSeed AssignContextConcern(ClassDeclaration roleClass) =>
        new(
            SetRoleProperty(roleClass),
            PointCut.AfterSet,
            FunctionType.Action
        );

    private static MethodDeclarationSyntax SetRoleProperty(ClassDeclaration roleClass) =>
        MethodDeclaration(
                PredefinedType(Token(SyntaxKind.VoidKeyword)),
                Identifier("dummy_function_identifier")
            )
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList<ParameterSyntax>(
                        Parameter(Identifier("v"))
                            .WithType(IdentifierName(roleClass.Identifier))
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