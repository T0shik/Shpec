using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

public class ClassTemplate
{
    public static MemberDeclarationSyntax Create(ClassSeed seed)
    {
        var classTokens = new[] { Token(SyntaxKind.PublicKeyword) }.AsEnumerable();
        if (seed.partial)
        {
            classTokens = classTokens.Append(Token(SyntaxKind.PartialKeyword));
        }

        var members = seed.properties.Select(PropertyTemplate.Create)
            .Concat(seed.classes.Select(Create));
        
        return ClassDeclaration(seed.Identifier)
            .WithModifiers(TokenList(classTokens))
            .WithMembers(List(members));
    }
}