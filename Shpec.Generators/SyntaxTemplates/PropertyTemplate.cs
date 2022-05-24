using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Shpec.Generators.Utils.Ops;

namespace Shpec.Generators.SyntaxTemplates;

class PropertyTemplate
{
    public static MemberDeclarationSyntax Create(PropertySeed seed)
    {
        return PropertyDeclaration(
                IdentifierName(seed.Type),
                Identifier(seed.Identifier))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithAccessorList(
                AccessorList(
                    List(new[]
                    {
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    })));
    }
}