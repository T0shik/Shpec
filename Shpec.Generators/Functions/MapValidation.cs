using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators.Functions;

internal static class MapValidation
{

    public static IReadOnlyCollection<ValidationSeed> Map(IReadOnlyCollection<BaseValidation> baseValidation)
    {
        List<ValidationSeed> result = new();

        foreach (var b in baseValidation)
        {
            var seed = b switch
            {
                AdHocValidation v => v.Exp switch
                {
                    SimpleLambdaExpressionSyntax e => new AdHocValidationSeed(e),
                    _ => throw new Exception("unknwon adhoc declaration scenario")
                },
                _ => throw new Exception("unknown validation declaration")
            };

            result.Add(seed);
        }

        return result.AsReadOnly();
    }
}
