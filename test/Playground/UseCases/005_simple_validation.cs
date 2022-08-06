using Shpec;
using Shpec.Declare;

namespace Playground.UseCases;


public class simple_validation : IUseCase
{
    public void Execute()
    {
        var e = new SomeEntity() { 
            Positive = 0,
            Negative = 0,
            PositiveOrZero = -1,
            NegativeOrZero = 1,
            IsZero = 1,
            NotZero = 0,
        };

        var validationResult = e.Valid();
        if (validationResult)
        {
            throw new Exception("not worky at all..");
        }

        if (validationResult.Errors.Count != typeof(ValidatedProperty).GetProperties().Length)
        {
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"Validation Error: {error}");
            }

            throw new Exception("missing validation errors");
        }

        Console.WriteLine($"--Validation Erros-- {validationResult}");
        foreach (var error in validationResult.Errors)
        {
            Console.WriteLine($"Validation Error: {error}");
        }
    }
}

public partial class SomeEntity
{
    Members _p => new(
        ValidatedProperty.Positive,
        ValidatedProperty.Negative,
        ValidatedProperty.PositiveOrZero,
        ValidatedProperty.NegativeOrZero,
        ValidatedProperty.IsZero,
        ValidatedProperty.NotZero
    );
}

public static class ValidatedProperty
{
    public static int Positive => Member<int>.Property().must(x => x > 0).must(x => x < 100);
    public static int Negative => Member<int>.Property().must(x => x < 0);
    public static int PositiveOrZero => Member<int>.Property().must(x => x >= 0);
    public static int NegativeOrZero => Member<int>.Property().must(x => x <= 0);
    public static int IsZero => Member<int>.Property().must(_ => _ == 0);
    public static int NotZero => Member<int>.Property().must(_ => _ != 0);
}