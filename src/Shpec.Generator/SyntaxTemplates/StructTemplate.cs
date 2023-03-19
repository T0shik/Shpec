using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class StructTemplate
{
    public static MemberDeclarationSyntax CreateStructDeclaration(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var classTokens = new List<SyntaxToken>() { Token(seed.Accessibility) };

        classTokens.Add(Token(SyntaxKind.PartialKeyword));

        var structDeclaration = StructDeclaration(seed.Identifier);

        List<MemberDeclarationSyntax> members = new();
        var ctor = ConstructorTemplate.CreateConstructor(seed);
        if (ctor != null)
        {
            members.Add(ctor);
        }

        members.AddRange(
            seed.Members.SelectMany(x => x switch
            {
                ComputedPropertySeed cps => ComputedPropertyTemplate.Create(cps),
                PropertySeed ps => PropertyTemplate.Create(ps),
                ClassSeed cs => new[] { ClassTemplate.Create(cs) },
                _ => throw new NotImplementedException("Unhandled Seed."),
            })
        );

        if (child != null)
        {
            members.Add(child);
        }

        foreach (var conversion in seed.Conversions)
        {
            members.Add(ConversionOperatorTemplate.Create(conversion));
        }

        var validationMember = ValidationMethodTemplate.Create(seed);
        if (validationMember != null)
        {
            members.Add(validationMember);
        }

        if (seed.Interfaces.Count > 0)
        {
            structDeclaration = structDeclaration.WithBaseList(InterfaceImplementationTemplate.Create(seed));
        }
        
        return structDeclaration
            .WithModifiers(TokenList(classTokens))
            .WithMembers(List(members));
    }
}