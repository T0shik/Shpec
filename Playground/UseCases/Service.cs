using Shpec;

namespace Playground.UseCases;

public partial class Service : IUseCase
{
    public void Execute()
    {
        new Service().Do(new() { FirstName = "Foo", LastName = "Bar", Age = 10 });
    }

    public partial class Person
    {
        _schema _s => _schema.define(
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
