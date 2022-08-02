using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

public static class MethodTemplate
{
    public static MethodDeclarationSyntax Transform(MethodDeclarationSyntax origin, BlockSyntax transformedBody)
    {
        return MethodDeclaration(origin.ReturnType, origin.Identifier)
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(transformedBody);
    }
}