using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generators.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class ClassTemplate
{
    public static MemberDeclarationSyntax Create(ClassSeed seed)
    {
        return Create(seed, null);
    }

    public static MemberDeclarationSyntax Create(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var declaration = seed switch
        {
            { Struct: true, Record: true } => RecordTemplate.CreateRecordDeclaration(seed, child),
            { Record: true } => RecordTemplate.CreateRecordDeclaration(seed, child),
            { Struct: true } => StructTemplate.CreateStructDeclaration(seed, child),
            { Record: false } => CreateClassDeclaration(seed, child),
            _ => throw new Exception(":(")
        };
        if (seed.Parent == null)
        {
            return declaration;
        }

        return Create(seed.Parent, declaration);
    }

    private static MemberDeclarationSyntax CreateClassDeclaration(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var classTokens = new List<SyntaxToken>() { Token(seed.Accessibility) };
        if (seed.Static)
        {
            classTokens.Add(Token(SyntaxKind.StaticKeyword));
        }

        classTokens.Add(Token(SyntaxKind.PartialKeyword));

        List<MemberDeclarationSyntax> members = new();

        var ctor = ConstructorTemplate.CreateConstructor(seed);
        if (ctor != null)
        {
            members.Add(ConstructorTemplate.CreateDefaultConstructor(seed));
            members.Add(ctor);
        }

        members.AddRange(
            seed.Members.SelectMany(x => x switch
            {
                ComputedPropertySeed cps => ComputedPropertyTemplate.Create(cps),
                PropertySeed ps => new() { PropertyTemplate.Create(ps) },
                ClassSeed cs => new() { Create(cs) },
                MethodSeed ms => new() { ms.Syntax },
                _ => throw new ShpecGenerationException($"Unhandled Seed in ClassTemplate.CreateClassDeclaration. Seed: {x}"),
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

        return ClassDeclaration(seed.Identifier)
            .WithModifiers(TokenList(classTokens))
            .WithMembers(List(members));
    }
}