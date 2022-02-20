using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators;

public class SchemaDeclarationAggregate : ISyntaxReceiver
{
    public readonly List<SchemaDeclaration> Schemas = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not FieldDeclarationSyntax fieldDeclaration)
        {
            return;
        }

        var attributeList = fieldDeclaration.DescendantNodes()
            .OfType<AttributeListSyntax>()
            .FirstOrDefault();

        if (attributeList == null)
        {
            return;
        }

        var declareSchemaPresent = attributeList.DescendantNodes()
            .OfType<AttributeSyntax>()
            .Any(x => x.Name.ToString() == "DeclareSchema");

        if (!declareSchemaPresent)
        {
            return;
        }

        var propertyArguments = fieldDeclaration.DescendantNodes(_ => true)
            .OfType<ArgumentSyntax>()
            .ToList();

        if (propertyArguments is not { Count: > 0 })
        {
            return;
        }

        var propertyNames = propertyArguments.Select(x => x.GetText().ToString()).ToImmutableArray();

        var classDeclaration = fieldDeclaration.GetParent<ClassDeclarationSyntax>();
        var namespaceDeclaration = fieldDeclaration.GetParent<FileScopedNamespaceDeclarationSyntax>();
        var variableDeclarator = fieldDeclaration.DescendantNodes(_ => true)
            .OfType<VariableDeclaratorSyntax>()
            .Single();

        Schemas.Add(new(
            namespaceDeclaration.Name.ToString(),
            classDeclaration.Identifier.Text,
            variableDeclarator.Identifier.Text,
            propertyNames
        ));
    }
}