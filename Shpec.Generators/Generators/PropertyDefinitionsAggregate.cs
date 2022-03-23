using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Generators;

public class PropertyDefinitionsAggregate : ISyntaxReceiver
{
    public List<PropertyDefinition> Definitions { get; set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax attributeSyntax)
        {
            return;
        }

        if (attributeSyntax.Name.ToString() != "PropertyDefinitions")
        {
            return;
        }

        var classDeclarationSyntax = attributeSyntax.GetParent<ClassDeclarationSyntax>();
        Definitions = classDeclarationSyntax.Members
            .Where(m => m is FieldDeclarationSyntax)
            .Select(m =>
            {
                var f = m as FieldDeclarationSyntax;
                var v = f.Declaration.Variables.First();
                var exp = v.Initializer.Value;

                return new PropertyDefinition(
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
}