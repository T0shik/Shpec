using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class RecordTemplate
{
    public static MemberDeclarationSyntax CreateRecordDeclaration(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var recordTokens = new List<SyntaxToken>() { Token(seed.Accessibility) };
        if (!seed.Struct && seed.Static)
        {
            recordTokens.Add(Token(SyntaxKind.StaticKeyword));
        }

        recordTokens.Add(Token(SyntaxKind.PartialKeyword));

        List<MemberDeclarationSyntax> members = new();

        members.AddRange(
            seed.Members
                .Where(x => x is ComputedPropertySeed or ClassSeed)
                .SelectMany(x => x switch
                {
                    ComputedPropertySeed cps => ComputedPropertyTemplate.Create(cps),
                    ClassSeed cs => new() { ClassTemplate.Create(cs) },
                    _ => throw new NotImplementedException("Unhandled seed in Record Template."),
                })
        );

        if (child != null)
        {
            members.Add(child);
        }

        // todo: record conversions
        // foreach (var conversion in seed.Conversions)
        // {
        //     members.Add(ConversionOperatorTemplate.Create(conversion));
        // }

        var validationMember = ValidationMethodTemplate.Create(seed);
        if (validationMember != null)
        {
            members.Add(validationMember);
        }

        var parameters = CreateParameters(seed.Members.OfType<PropertySeed>());

        var declaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword), Identifier(seed.Identifier))
            .WithModifiers(TokenList(recordTokens));

        if (seed.Struct)
        {
            declaration = declaration.WithClassOrStructKeyword(
                Token(SyntaxKind.StructKeyword));
        }

        return declaration.WithParameterList(ParameterList(parameters))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithMembers(List(members))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
    }

    private static SeparatedSyntaxList<ParameterSyntax> CreateParameters(IEnumerable<PropertySeed> properties)
    {
        var parameters = new List<SyntaxNodeOrToken>();
        foreach (var prop in properties)
        {
            parameters.Add(
                Parameter(Identifier(prop.Identifier))
                    .WithType(prop.Type)
            );
            parameters.Add(Token(SyntaxKind.CommaToken));
        }

        parameters.RemoveAt(parameters.Count - 1);

        return SeparatedList<ParameterSyntax>(parameters);
    }
}