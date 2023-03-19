using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Utils;
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

        var (parameters, members) = ResolveMembers(seed);

        if (child != null)
        {
            members = members.Add(child);
        }

        // todo: record conversions
        // foreach (var conversion in seed.Conversions)
        // {
        //     members.Add(ConversionOperatorTemplate.Create(conversion));
        // }

        var validationMember = ValidationMethodTemplate.Create(seed);
        if (validationMember != null)
        {
            members = members.Add(validationMember);
        }

        var declaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword), Identifier(seed.Identifier))
            .WithModifiers(TokenList(recordTokens));

        if (seed.Struct)
        {
            declaration = declaration.WithClassOrStructKeyword(
                Token(SyntaxKind.StructKeyword));
        }

        if (seed.Interfaces.Count > 0)
        {
            declaration = declaration.WithBaseList(InterfaceImplementationTemplate.Create(seed));
        }

        return declaration
            .WithParameterList(
                ParameterList(CreateParametersSyntax(parameters))
            )
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithMembers(List(members))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
    }

    private record RecordMembers(ImmutableList<PropertySeed> Parameters, ImmutableList<MemberDeclarationSyntax> BodyMembers);

    private static RecordMembers ResolveMembers(ClassSeed seed)
    {
        List<PropertySeed> Parameters = new();
        List<MemberDeclarationSyntax> BodyMembers = new();
        foreach (var member in seed.Members)
        {
            if (member is ComputedPropertySeed cps)
            {
                BodyMembers.AddRange(ComputedPropertyTemplate.Create(cps));
            }
            else if (member is ClassSeed cs)
            {
                BodyMembers.Add(ClassTemplate.Create(cs));
            }
            else if (member is PropertySeed { DeclarationSpecificToInterface: true } interfaceSpecificProperty)
            {
                BodyMembers.AddRange(PropertyTemplate.Create(interfaceSpecificProperty));
            }
            else if (member is PropertySeed { DeclarationSpecificToInterface: false } ps)
            {
                Parameters.Add(ps);
            }
            else
            {
                throw new ShpecTranslationException("unrecognized properties");
            }
        }

        return new RecordMembers(
            Parameters.ToImmutableList(),
            BodyMembers.ToImmutableList()
        );
    }

    private static SeparatedSyntaxList<ParameterSyntax> CreateParametersSyntax(IEnumerable<PropertySeed> properties)
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