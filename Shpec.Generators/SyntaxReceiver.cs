using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shpec.Generators.Generators;

class SyntaxReceiver : ISyntaxReceiver
{
    public SyntaxReceiver()
    {
        propertyDeclarations = new();
        computedPropertyDeclarations = new();
        definitionsAggregate = new();

        _syntaxGenerators = new ISyntaxReceiver[] 
        {
            propertyDeclarations,
            computedPropertyDeclarations,
            definitionsAggregate,
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

    public PropertyDeclarationsAggregate propertyDeclarations { get; }
    public ComputedPropertyDeclarationsAggregate computedPropertyDeclarations { get; }
    public DefinitionsAggregate definitionsAggregate { get; }
    public List<Declaration> Declarations => definitionsAggregate.Definitions.Values.ToList();
}