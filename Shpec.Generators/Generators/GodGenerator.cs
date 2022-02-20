using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shpec.Generators;

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

    public SchemaDeclarationAggregate Declarations => (SchemaDeclarationAggregate)SyntaxGenerators.FirstOrDefault(x => x is SchemaDeclarationAggregate);
    public SpecDefinitionAggregate Definitions => (SpecDefinitionAggregate)SyntaxGenerators.FirstOrDefault(x => x is SpecDefinitionAggregate);
}