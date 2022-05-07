using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Shpec.Generators.Utils.Ops;

namespace Shpec.Generators.SyntaxTemplates;

class PropertyTemplate
{
    public static MemberDeclarationSyntax Create(PropertySeed seed)
    {
        // var propertyType = seed.Type.Assert(
        //     SyntaxKind.BoolKeyword,
        //     SyntaxKind.ByteKeyword,
        //     SyntaxKind.SByteKeyword,
        //     SyntaxKind.ShortKeyword,
        //     SyntaxKind.UShortKeyword,
        //     SyntaxKind.IntKeyword,
        //     SyntaxKind.UIntKeyword,
        //     SyntaxKind.LongKeyword,
        //     SyntaxKind.ULongKeyword,
        //     SyntaxKind.DoubleKeyword,
        //     SyntaxKind.FloatKeyword,
        //     SyntaxKind.DecimalKeyword,
        //     SyntaxKind.StringKeyword,
        //     SyntaxKind.CharKeyword,
        //     SyntaxKind.ObjectKeyword
        // );

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