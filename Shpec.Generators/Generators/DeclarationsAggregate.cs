using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Generators;

public class DeclarationsAggregate : ISyntaxReceiver
{
    public readonly List<Declaration> Declarations = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax attributeSyntax)
        {
            return;
        }

        if (attributeSyntax.Name.ToString() != "Schema")
        {
            return;
        }

        var fieldDeclaration = attributeSyntax.GetParent<FieldDeclarationSyntax>();

        var propertyArguments = fieldDeclaration.DescendantNodes(_ => true)
            .OfType<ArgumentSyntax>()
            .ToList();

        if (propertyArguments is not { Count: > 0 })
        {
            return;
        }

        var namespaceDeclaration = fieldDeclaration.GetParent<FileScopedNamespaceDeclarationSyntax>();
        var classDeclaration = fieldDeclaration.GetParent<ClassDeclarationSyntax>();
        var propertyNames = propertyArguments.Select(x => x.GetText().ToString()).ToImmutableArray();

        var variableDeclarator = fieldDeclaration.DescendantNodes(_ => true)
            .OfType<VariableDeclaratorSyntax>()
            .Single();
        
        

        Declarations.Add(new(
            namespaceDeclaration.Name.ToString(),
            classDeclaration.Identifier.ToString(),
            propertyNames
        ));
    }
}