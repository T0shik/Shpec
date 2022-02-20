using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators;

public class SpecDefinitionAggregate : ISyntaxReceiver
{
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax attributeSyntax)
        {
            return;
        }

        if (attributeSyntax.Name.ToString() != "SpecDefinition")
        {
            return;
        }

        var classDeclarationSyntax = attributeSyntax.GetParent<ClassDeclarationSyntax>();
        PropertyDefinitions = classDeclarationSyntax.Members
            .Where(m => m is FieldDeclarationSyntax)
            .Select(m =>
            {
                var f = m as FieldDeclarationSyntax;
                var v = f.Declaration.Variables.First();
                var exp = v.Initializer.Value;

                return new PropertyInfo(
                    ((MemberAccessExpressionSyntax) exp).Name.ToString(),
                    v.Identifier.Text,
                    exp switch
                    {
                        MemberAccessExpressionSyntax ma => ma.Name.ToString() switch
                        {
                            "String" => SyntaxKind.StringKeyword,
                            "Int" => SyntaxKind.IntKeyword,
                            _ => SyntaxKind.StringKeyword,
                        },
                        _ => SyntaxKind.StringKeyword,
                    }
                );
            })
            .ToList();
    }

    public List<PropertyInfo> PropertyDefinitions { get; set; }
}