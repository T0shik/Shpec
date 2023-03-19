using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class InterfaceTemplate
{
    internal static InterfaceDeclarationSyntax Create(RoleSeed seed)
    {
        var members = new List<MemberDeclarationSyntax>();

        members.AddRange(seed.Members.SelectMany(MemberTemplate.Create));

        return InterfaceDeclaration(seed.Identifier)
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.PartialKeyword)
                )
            )
            .WithMembers(List(members));
    }
}