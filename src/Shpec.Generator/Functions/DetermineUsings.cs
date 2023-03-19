using System.Collections.Immutable;
using System.Numerics;
using Shpec.Generator.Utils;

namespace Shpec.Generator.Functions;

internal static class DetermineUsings
{
    internal static ImmutableHashSet<string> From(IReadOnlyCollection<Seed> properties)
    {
        HashSet<string> result = new();

        foreach (var p in properties.OfType<PropertySeed>())
        {
            if (p.Type.ToString().LastAccessor() == nameof(BigInteger))
            {
                result.Add("System.Numerics");
            }
            else if (p.Type.ToString().LastAccessor() == nameof(Guid))
            {
                result.Add("System");
            }
        }

        return result.ToImmutableHashSet();
    }
}