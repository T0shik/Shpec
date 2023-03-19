using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class ClassTemplate
{
    public static MemberDeclarationSyntax Create(TypeSeed seed)
    {
        return Create(seed, null);
    }

    public static MemberDeclarationSyntax Create(TypeSeed seed, MemberDeclarationSyntax? child)
    {
        var declaration = seed switch
        {
            ClassSeed { Struct: true, Record: true } cs => RecordTemplate.CreateRecordDeclaration(cs, child),
            ClassSeed { Record: true } cs => RecordTemplate.CreateRecordDeclaration(cs, child),
            ClassSeed { Struct: true } cs => StructTemplate.CreateStructDeclaration(cs, child),
            ClassSeed { Record: false } cs => CreateClassDeclaration(cs, child),
            RoleSeed rs => InterfaceTemplate.Create(rs),
            _ => throw new ShpecGenerationException("failed to translate class branch")
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

        var classDeclaration = ClassDeclaration(seed.Identifier)
            .WithModifiers(TokenList(classTokens));

        if (seed.Interfaces.Count > 0)
        {
            classDeclaration = classDeclaration.WithBaseList(InterfaceImplementationTemplate.Create(seed));
        }

        List<MemberDeclarationSyntax> members = new();

        if (seed.CtorByDefault)
        {
            var ctor = ConstructorTemplate.CreateConstructor(seed);
            if (ctor != null)
            {
                members.Add(ConstructorTemplate.CreateDefaultConstructor(seed));
                members.Add(ctor);
            }
        }

        members.AddRange(seed.Members.SelectMany(MemberTemplate.Create));

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
        
        classDeclaration = classDeclaration.WithMembers(List(members));
        
        if (validationMember != null)
        {
            return ValidationBaseTemplate.AddTo(classDeclaration);
        }

        return classDeclaration;
    }
}