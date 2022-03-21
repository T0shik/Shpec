using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shpec.Generators.Generators;

public class GodGenerator : ISyntaxReceiver
{
    public GodGenerator()
    {
        var syntaxReceiver = typeof(ISyntaxReceiver);
        var godType = typeof(GodGenerator);
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
    public DeclarationsAggregate Declarations => (DeclarationsAggregate)SyntaxGenerators.FirstOrDefault(x => x is DeclarationsAggregate);
}