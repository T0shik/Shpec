using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class ConstructorTemplate
{
    public static MemberDeclarationSyntax CreateDefaultConstructor(ClassSeed seed)
    {
        return ConstructorDeclaration(Identifier(seed.Identifier))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(Block());
    }


    public static MemberDeclarationSyntax? CreateConstructor(ClassSeed seed)
    {
        var parameters = new List<SyntaxNodeOrToken>();
        var expressions = new List<ExpressionStatementSyntax>();

        var properties = seed.Members
            .Where(x => x is PropertySeed and not ComputedPropertySeed)
            .Select(x => x as PropertySeed)
            .ToList();

        if (properties.Count == 0)
        {
            return null;
        }

        foreach (var propertySeed in properties)
        {
            parameters.Add(
                Parameter(Identifier(propertySeed.Identifier))
                    .WithType(IdentifierName(propertySeed.Type))
            );

            parameters.Add(
                Token(SyntaxKind.CommaToken)
            );

            expressions.Add(
                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(propertySeed.Identifier)
                        ),
                        IdentifierName(propertySeed.Identifier)
                    )
                )
            );
        }

        parameters.RemoveAt(parameters.Count - 1);

        return ConstructorDeclaration(Identifier(seed.Identifier))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(parameters)))
            .WithBody(Block(expressions));
    }
}