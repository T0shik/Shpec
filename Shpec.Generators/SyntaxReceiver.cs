using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shpec.Generators.Generators;

class SyntaxReceiver : ISyntaxReceiver
{
    public SyntaxReceiver()
    {
        var syntaxReceiver = typeof(ISyntaxReceiver);
        var godType = typeof(SyntaxReceiver);
        SyntaxGenerators = godType.Assembly.GetTypes()
            .Where(x => x.GetInterfaces().Any(i => i == syntaxReceiver) && x != godType)
            .Select(t => (ISyntaxReceiver)Activator.CreateInstance(t))
            .ToImmutableArray();
    }

    public readonly IReadOnlyCollection<ISyntaxReceiver> SyntaxGenerators;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        foreach (var generator in SyntaxGenerators)
        {
            generator.OnVisitSyntaxNode(syntaxNode);
        }
    }

    public PropertyDefinitionsAggregate PropertyDefinitions => (PropertyDefinitionsAggregate)SyntaxGenerators.FirstOrDefault(x => x is PropertyDefinitionsAggregate);
    public List<Declaration> Declarations => ((DeclarationsAggregate)SyntaxGenerators.FirstOrDefault(x => x is DeclarationsAggregate)).Declarations.Values.ToList();
}