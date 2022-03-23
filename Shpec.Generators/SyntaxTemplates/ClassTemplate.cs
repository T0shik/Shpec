using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class ClassTemplate
{
    public static MemberDeclarationSyntax Create(ClassSeed seed)
    {
        var classTokens = new[] { Token(seed.accessibility) }.AsEnumerable();
        if (seed.partial)
        {
            classTokens = classTokens.Append(Token(SyntaxKind.PartialKeyword));
        }

        var members = seed.properties.Select(PropertyTemplate.Create)
            .Concat(seed.classes.Select(Create));
        
        return ClassDeclaration(seed.identifier)
            .WithModifiers(TokenList(classTokens))
            .WithMembers(List(members));
    }
}