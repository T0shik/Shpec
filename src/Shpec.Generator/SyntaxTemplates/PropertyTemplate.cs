using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

static class PropertyTemplate
{
    public static IEnumerable<MemberDeclarationSyntax> Create(PropertySeed seed)
    {
        if (seed.Concerns.Count == 0)
        {
            yield return StandaloneProperty(seed);
        }
        else
        {
            foreach (var member in WithField(seed))
            {
                yield return member;
            }
        }
    }

    private static MemberDeclarationSyntax StandaloneProperty(PropertySeed seed)
    {
        var setter = seed switch
        {
            { Immutable: true } => SyntaxKind.InitAccessorDeclaration,
            _ => SyntaxKind.SetAccessorDeclaration,
        };

        return PropertyDeclaration(IdentifierName(seed.Type), Identifier(seed.Identifier))
            .WithModifiers(seed)
            .WithAccessorList(
                AccessorList(
                    List(new[]
                    {
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(setter)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    })));
    }

    public static IEnumerable<MemberDeclarationSyntax> WithField(PropertySeed seed)
    {
        var fieldIdentifier = $"__{seed.Identifier.ToLower()}";
        var statements = new List<StatementSyntax>();
        
        foreach (var seedConcern in seed.Concerns.Where(x => x.PointCut == PointCut.BeforeSet))
        {
            statements.AddRange(InlineConcernTemplate.InlineSetterConcerns(seedConcern, fieldIdentifier));
        }
        
        // final assignment to field; will need to change if function concern is present
        statements.Add(
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(fieldIdentifier),
                    IdentifierName("value")))
        );

        foreach (var seedConcern in seed.Concerns.Where(x => x.PointCut == PointCut.AfterSet))
        {
            statements.AddRange(InlineConcernTemplate.InlineSetterConcerns(seedConcern, fieldIdentifier));
        }

        return new MemberDeclarationSyntax[]
        {
            FieldDeclaration(VariableDeclaration(IdentifierName(seed.Type))
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(fieldIdentifier)))))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword))),

            PropertyDeclaration(IdentifierName(seed.Type), Identifier(seed.Identifier))
                .WithModifiers(seed)
                .WithAccessorList(
                    AccessorList(
                        List(
                            new[]
                            {
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            IdentifierName(fieldIdentifier)))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                        SyntaxKind.SetAccessorDeclaration)
                                    .WithBody(Block(statements)),
                            })))
        };
    }

    private static PropertyDeclarationSyntax WithModifiers(this PropertyDeclarationSyntax property, PropertySeed seed)
    {
        if (seed.DeclarationSpecificToInterface)
        {
            return property;
        }

        return property.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
    }
}