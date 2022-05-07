using System.Numerics;

namespace Shpec.Generators.Functions;

internal static class DetermineUsings
{
    internal static IReadOnlyCollection<string> From(IReadOnlyCollection<PropertySeed> properties)
    {
        List<string> result = new();

        foreach (var p in properties)
        {
            if (p.Type == nameof(BigInteger))
            {
                result.Add("System.Numerics");
            }
        }


        return result.AsReadOnly();
    }
}