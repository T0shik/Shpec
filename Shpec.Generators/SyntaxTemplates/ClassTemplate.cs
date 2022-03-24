using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class ClassTemplate
{
    public static MemberDeclarationSyntax Create(ClassSeed seed)
    {
        return Create(seed, null);
    }

    private static MemberDeclarationSyntax Create(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var declaration = CreateClassDeclaration(seed, child);
        return seed.Parent switch
        {
            { } s => CreateClassDeclaration(s, declaration),
            null => declaration,
        };
    }

    private static MemberDeclarationSyntax CreateClassDeclaration(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var classTokens = new[] { Token(seed.Accessibility), Token(SyntaxKind.PartialKeyword) }.AsEnumerable();
        if (seed.Static)
        {
            classTokens = classTokens.Append(Token(SyntaxKind.StaticKeyword));
        }

        var members = seed.Properties.Select(PropertyTemplate.Create)
            .Concat(seed.Classes.Select(Create));

        if (child != null)
        {
            members = members.Append(child);
        }

        return ClassDeclaration(seed.Identifier)
            .WithModifiers(TokenList(classTokens))
            .WithMembers(List(members));
    }
}
