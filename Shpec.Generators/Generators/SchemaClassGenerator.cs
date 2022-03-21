using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.Generators;

public class SchemaClassGenerator
{
    public SchemaClassGenerator(
        string nameSpace,
        string parentName,
        string className,
        IEnumerable<PropertySeed> properties
    )
    {
        Name = $"{nameSpace}{parentName}{className}";
        Source = CompilationUnit()
            .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                    FileScopedNamespaceDeclaration(
                            IdentifierName(nameSpace))
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration(parentName)
                                    .WithModifiers(
                                        TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                                    .WithMembers(
                                        SingletonList<MemberDeclarationSyntax>(
                                            ClassDeclaration(className.Replace("Schema", ""))
                                                .WithModifiers(
                                                    TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
                                                .WithMembers(
                                                    List(
                                                        MemberList(properties)
                                                    ))))))))
            .NormalizeWhitespace()
            .GetText(Encoding.UTF8);
    }

    private MemberDeclarationSyntax[] MemberList(IEnumerable<PropertySeed> properties)
    {
        return properties
            .Select(p => PropertyDeclaration(
                    PredefinedType(
                        Token(p.ValueType)),
                    Identifier(p.Identifier))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(
                    AccessorList(
                        List(
                            new[]
                            {
                                AccessorDeclaration(
                                        SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                        SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken))
                            }))))
            .ToArray();
    }

    public SourceText Source { get; }
    public string Name { get; }
}