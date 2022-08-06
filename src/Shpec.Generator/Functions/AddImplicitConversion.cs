using System.Collections.Immutable;

namespace Shpec.Generator.Functions;

internal class AddImplicitConversion
{
    private readonly List<PropertyDefinition> _propertyDefinitions;

    public AddImplicitConversion(List<PropertyDefinition> propertyDefinitions)
    {
        _propertyDefinitions = propertyDefinitions;
    }

    public NamespaceSeed To(NamespaceSeed target, NamespaceSeed from)
    {
        var commonProperties = GetCommonProperties(target.Clazz, from.Clazz).ToList();

        if (commonProperties.Count == 0)
        {
            return target;
        }

        IReadOnlyCollection<string> propSeeds;

        propSeeds = _propertyDefinitions
        .Where(x => commonProperties.Contains(x.Identifier))
        .Select(x => x.Identifier)
        .ToList()
        .AsReadOnly();

        return target with
        {
            Clazz = target.Clazz with
            {
                Conversions = ImmutableArray<ConversionSeed>.Empty
                       .AddRange(target.Clazz.Conversions)
                       .Add(new(target, from, propSeeds)),
            }
        };
    }

    private static IEnumerable<string> GetCommonProperties(ClassSeed target, ClassSeed from)
    {
        foreach (var member in target.Members)
        {
            if (member is not PropertySeed fp)
            {
                continue;
            }

            var match = from.Members.OfType<PropertySeed>().FirstOrDefault(x => fp.Identifier == x.Identifier);
            if (match != null)
            {
                yield return match.Identifier;
            }
        }
    }
}