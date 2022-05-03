using Shpec;

namespace Playground.UseCases;


public class simple_validation : IUseCase
{
    public void Execute()
    {
        var e = new SomeEntity() { FirstName = "Foo", NumericIdentifier = 0 };
        var validationResult = e.Valid();
        if (validationResult)
        {
            throw new Exception("this ain't working");
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
    _s _s => _s.define(
        Property.FirstName,
        Property.NumericIdentifier
        );
}