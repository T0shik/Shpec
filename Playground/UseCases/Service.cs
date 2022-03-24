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
        _schema _s => _schema.declare(
                Property.FirstName,
                Property.LastName,
                Property.Age
            );

        public string FullName => FirstName + " " + LastName;
        public string Introduce => $"Hello, my name is {FirstName} and I am {Age} year's old.";
    }

    public void Do(Person person)
    {
        Console.WriteLine(person.FullName);
        Console.WriteLine(person.Introduce);
    }
}
