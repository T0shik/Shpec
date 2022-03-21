using Shpec;
using static Playground.GenericProperties;

namespace Playground;

public partial class Service
{
    [Schema]
    public Schema PersonSchema = Schema.Define(FirstName, LastName, Age);

    public partial class Person
    {
        public string FullName => FirstName + " " + LastName;
        public string Introduce => $"Hello, my name is {FirstName} and I am {Age} year's old.";
    }

    public void Do(Person person)
    {
        Console.WriteLine(person.FullName);
        Console.WriteLine(person.Introduce);
    }
}
