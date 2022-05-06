using Shpec;
using static Shpec.Declare;

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

        if (validationResult.Errors.Length != typeof(ValidatedProperty).GetProperties().Length)
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
    Properties _p => new(
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
    public static int Positive => _property<int>().must(x => x > 0);
    public static int Negative => _property<int>().must(x => x < 0);
    public static int PositiveOrZero => _property<int>().must(x => x >= 0);
    public static int NegativeOrZero => _property<int>().must(x => x <= 0);
    public static int IsZero => _property<int>().must(_ => _ == 0);
    public static int NotZero => _property<int>().must(_ => _ != 0);
}