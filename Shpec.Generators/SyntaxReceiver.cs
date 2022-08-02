using Microsoft.CodeAnalysis;
using Shpec.Generators.Aggregators;

namespace Shpec.Generators;

class SyntaxReceiver : ISyntaxReceiver
{
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        AggregatePropertyDefinitions.OnVisitSyntaxNode(syntaxNode);
        AggregateComputedPropertyDefinitions.OnVisitSyntaxNode(syntaxNode);
        AggregateMethodDefinitions.OnVisitSyntaxNode(syntaxNode);
        AggregateUsages.OnVisitSyntaxNode(syntaxNode);
    }

    public AggregatePropertyDefinitions AggregatePropertyDefinitions { get; } = new();
    public AggregateComputedPropertyDefinitions AggregateComputedPropertyDefinitions { get; } = new();
    public AggregateMethodDefinitions AggregateMethodDefinitions { get; } = new();
    public AggregateUsages AggregateUsages { get; } = new();
    public List<Usage> Usages => AggregateUsages.Captures.Values.ToList();
}