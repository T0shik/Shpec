using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class ClassTemplate
{
    public static MemberDeclarationSyntax Create(ClassSeed seed)
    {
        var classTokens = new[] { Token(seed.Accessibility) }.AsEnumerable();
        if (seed.Partial)
        {
            classTokens = classTokens.Append(Token(SyntaxKind.PartialKeyword));
        }

        var members = seed.Properties.Select(PropertyTemplate.Create)
            .Concat(seed.Classes.Select(Create));
        
        return ClassDeclaration(seed.Identifier)
            .WithModifiers(TokenList(classTokens))
            .WithMembers(List(members));
    }
}