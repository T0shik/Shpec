using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Shpec.Generators.Functions;

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

        var propSeeds = _propertyDefinitions
            .Where(x => commonProperties.Contains(x.Identifier))
            .Select(x => new PropertySeed(x.Identifier, x.Type))
            .ToList()
            .AsReadOnly();


        return target with
        {
            Clazz = target.Clazz with
            {
                Conversions = new(target.Clazz.Conversions.Append(new(target, from, propSeeds)).ToList()),
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