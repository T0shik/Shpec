using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Shpec.Generator.Utils;
using IdentifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax;

namespace Shpec.Generator.Aggregators;

class AggregateComputedPropertyDefinitions : ISyntaxReceiver
{
    public List<ComputedPropertyDefinition> Captures { get; set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not MemberAccessExpressionSyntax { Expression: GenericNameSyntax { Identifier.Text: "Member" }, Name.Identifier.Text: "Computed" } mae)
        {
            return;
        }

        var invocation = mae.GetParent<InvocationExpressionSyntax>();

        var propertyDeclarationSyntax = invocation
            .GetParent<PropertyDeclarationSyntax>();

        var identifier = propertyDeclarationSyntax.Identifier.ToString();
        var argument = invocation.ArgumentList.Arguments.First();
        
        Captures.Add(new(
            identifier,
            propertyDeclarationSyntax.Type.ToString(),
            ImmutableArray<BaseValidation>.Empty,
            argument.Expression
        ));
    }
}