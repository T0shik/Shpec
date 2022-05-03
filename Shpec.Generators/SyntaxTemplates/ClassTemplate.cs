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

        List<MemberDeclarationSyntax> members = new();

        members.AddRange(
            seed.Members.SelectMany(x => x switch
            {
                ComputedPropertySeed cps => ComputedPropertyTemplate.Create(cps),
                PropertySeed ps => new() { PropertyTemplate.Create(ps) },
                ClassSeed cs => new() { Create(cs) },
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

        var validationMember = CreateValidationMember(seed);
        if (validationMember != null)
        {
            members.Add(validationMember);
        }

        return ClassDeclaration(seed.Identifier)
            .WithModifiers(TokenList(classTokens))
            .WithMembers(List(members));
    }

    public static MemberDeclarationSyntax? CreateValidationMember(ClassSeed classSeed)
    {
        List<PropertySeed> properties = new();


        foreach (var m in classSeed.Members)
        {
            if (m is PropertySeed { Validations: { Count: > 0 } } ps)
            {
                properties.Add(ps);
            }
        }


        if (properties.Count == 0)
        {
            return null;
        }

        return ValidationMethodTemplate.Create(properties.AsReadOnly());
    }
}