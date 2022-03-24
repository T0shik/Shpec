using Shpec;

namespace Playground;

public partial class Service
{
    public partial class Person
    {
        [Schema] Schema _s = Schema.Define(
            GenericProperties.FirstName,
            GenericProperties.LastName,
            GenericProperties.Age
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