using System.Numerics;

namespace Shpec.Generator.Functions;

internal static class DetermineUsings
{
    internal static IReadOnlyCollection<string> From(IReadOnlyCollection<Seed> properties)
    {
        HashSet<string> result = new();

        foreach (var p in properties.OfType<PropertySeed>())
        {
            if (p.Type == nameof(BigInteger))
            {
                result.Add("System.Numerics");
            }
            else if (p.Type == nameof(Guid))
            {
                result.Add("System");
            }
        }


        return result.ToList().AsReadOnly();
    }
}