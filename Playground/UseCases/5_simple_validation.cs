using Shpec;

namespace Playground.UseCases;


public class simple_validation : IUseCase
{
    public void Execute()
    {
        var e = new SomeEntity() { FirstName = "Foo", NumericIdentifier = 0 };
        if (e.Valid())
        {
            throw new Exception("this ain't working");
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