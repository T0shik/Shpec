using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

public static class MethodTemplate
{
    public static MethodDeclarationSyntax Transform(MethodDeclarationSyntax origin, BlockSyntax transformedBody)
    {
        return MethodDeclaration(origin.ReturnType, origin.Identifier)
            .WithParameterList(origin.ParameterList)
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBody(transformedBody);
    }
}