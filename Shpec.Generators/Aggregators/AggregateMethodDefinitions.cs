using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generators.Utils;

namespace Shpec.Generators.Aggregators;

public class AggregateMethodDefinitions : ISyntaxReceiver
{
    public Dictionary<string, MethodDeclarationSyntax> Captures { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax { Name: IdentifierNameSyntax { Identifier.Text: "MethodDefinition" } } attr)
        {
            return;
        }

        var methodDeclaration = attr.GetParent<MethodDeclarationSyntax>();

        var identifier = methodDeclaration.Identifier.Text;
        if (methodDeclaration.Body == null)
        {
            throw new ShpecAggregationException($"failed to find method block for {identifier}", methodDeclaration);
        }

        Captures[identifier] = methodDeclaration;
    }
}