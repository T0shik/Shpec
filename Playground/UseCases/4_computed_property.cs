using Shpec;

namespace Playground.UseCases;

public partial class computed_property : IUseCase
{
    _s _s => _s.define(
            Property.FirstName,
            Property.LastName,
            Property.Age,
            Computed.BirthYear,
            Computed.FullName,
            Computed.Initials
        );

    public void Execute()
    {
        var c = new computed_property() { FirstName = "Foo", LastName = "Bar", Age = 55 };
        Console.WriteLine(c.BirthYear);
        Console.WriteLine(c.FullName);
        Console.WriteLine(c.Initials);
    }

}
