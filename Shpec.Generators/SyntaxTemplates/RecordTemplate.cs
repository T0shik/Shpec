using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class RecordTemplate
{
    public static MemberDeclarationSyntax Create(ClassSeed seed)
    {
        return Create(seed, null);
    }

    public static MemberDeclarationSyntax Create(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var declaration = CreateClassDeclaration(seed, child);
        return seed.Parent switch
        {
            { Record: true } parent => Create(parent, declaration),
            { Record: false } parent => ClassTemplate.Create(parent, declaration),
            null => declaration,
        };
    }


    private static MemberDeclarationSyntax CreateClassDeclaration(ClassSeed seed, MemberDeclarationSyntax? child)
    {
        var recordTokens = new List<SyntaxToken>() { Token(seed.Accessibility) };
        if (seed.Static)
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
                    ClassSeed cs => new() { Create(cs) },
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
        
        return RecordDeclaration(Token(SyntaxKind.RecordKeyword), Identifier(seed.Identifier))
            .WithModifiers(TokenList(recordTokens))
            .WithParameterList(ParameterList(parameters))
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
                    .WithType(IdentifierName(prop.Type))
            );
            parameters.Add(Token(SyntaxKind.CommaToken));
        }

        parameters.RemoveAt(parameters.Count - 1);

        return SeparatedList<ParameterSyntax>(parameters);
    }
}