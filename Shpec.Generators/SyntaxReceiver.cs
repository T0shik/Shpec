using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shpec.Generators.Generators;

class SyntaxReceiver : ISyntaxReceiver
{
    public SyntaxReceiver()
    {
        PropertyDefinitions = new();
        ComputedPropertyDefinitions = new();
        DeclarationsAggregate = new();

        _syntaxGenerators = new ISyntaxReceiver[] 
        {
            PropertyDefinitions,
            ComputedPropertyDefinitions,
            DeclarationsAggregate,
        };
    }

    private readonly IReadOnlyCollection<ISyntaxReceiver> _syntaxGenerators;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        foreach (var generator in _syntaxGenerators)
        {
            generator.OnVisitSyntaxNode(syntaxNode);
        }
    }

    public PropertyDefinitionsAggregate PropertyDefinitions { get; }
    public ComputedPropertyDefinitionsAggregate ComputedPropertyDefinitions { get; }
    public DeclarationsAggregate DeclarationsAggregate { get; }
    public List<Declaration> Declarations => DeclarationsAggregate.Declarations.Values.ToList();
}