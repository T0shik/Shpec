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
        if (target.Clazz is not ClassSeed targetC || from.Clazz is not ClassSeed fromC)
        {
            return target;
        }

        var commonProperties = GetCommonProperties(targetC, fromC).ToList();

        if (commonProperties.Count == 0)
        {
            return target;
        }

        IReadOnlyCollection<string> propSeeds = _propertyDefinitions
            .Where(x => commonProperties.Contains(x.Identifier))
            .Select(x => x.Identifier)
            .ToList();
        
        return target with
        {
            Clazz = targetC with
            {
                Conversions = targetC.Conversions
                    .Add(new(target, from, propSeeds)),
            }
        };
    }

    private static IEnumerable<string> GetCommonProperties(ClassSeed target, ClassSeed from)
    {
        foreach (var member in target.Members)
        {
            if (member is not PropertySeed { IncludeInCtor: true } fp)
            {
                continue;
            }

            var match = from.Members.OfType<PropertySeed>().FirstOrDefault(x => fp.Identifier == x.Identifier);
            if (match is { IncludeInCtor: true })
            {
                yield return match.Identifier;
            }
        }
    }
}