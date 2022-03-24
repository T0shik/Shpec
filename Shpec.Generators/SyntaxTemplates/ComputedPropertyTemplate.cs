using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class ComputedPropertyTemplate
{
    public static MemberDeclarationSyntax Create(ComputedPropertySeed seed)
    {
        return PropertyDeclaration( PredefinedType(Token(seed.Type)), Identifier(seed.Identifier) )
            .WithModifiers(TokenList( Token(SyntaxKind.PublicKeyword) ))
            .WithExpressionBody( ArrowExpressionClause(seed.Expression) )
            .WithSemicolonToken( Token(SyntaxKind.SemicolonToken) );
    }
}