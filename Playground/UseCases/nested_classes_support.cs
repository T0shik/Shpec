using Shpec;

namespace Playground.UseCases;

public partial class nested_classes_support : IUseCase
{
    public void Execute()
    {
        new nested_classes_support().Do(new() { FirstName = "Foo", LastName = "Bar", Age = 10 });
    }

    public partial class Person
    {
        _s _s => _s.define(
                Property.FirstName,
                Property.LastName,
                Property.Age,
                Computed.BirthYear,
                Computed.FullName
                //Computed.Introduce,
                //Computed.Initials
            );
    }

    public void Do(Person person)
    {
        Console.WriteLine(person.BirthYear);
        Console.WriteLine(person.FullName);
        //Console.WriteLine(person.Introduce);
        //Console.WriteLine(person.Initials);
    }
}
