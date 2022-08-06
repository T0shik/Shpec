using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class PropertyTemplate
{
    public static MemberDeclarationSyntax Create(PropertySeed seed)
    {
        var setter = seed switch
        {
            { Immutable: true } => SyntaxKind.InitAccessorDeclaration,
            _ => SyntaxKind.SetAccessorDeclaration,
        };

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
                        AccessorDeclaration(setter)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    })));
    }
}