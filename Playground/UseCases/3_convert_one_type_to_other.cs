using Shpec;

namespace Playground.UseCases;

public partial class convert_one_type_to_other : IUseCase
{
    public partial class PersonOne
    {
        _s _p => _s.define(
            Property.FirstName,
            Property.LastName,
            Property.Age
        );
    }

    public partial class PersonTwo
    {
        _s _p => _s.define(
            Property.FirstName,
            Property.LastName,
            Computed.FullName
        );
    }

    public void Execute()
    {
        PersonTwo two = new PersonOne() { FirstName = "First", LastName = "Last", Age = 10 };
        Console.WriteLine(two.FullName);
    }
}

public partial class convert_dog_to_cat : IUseCase
{
    public partial class Dog
    {
        _s _p => _s.define(
            Property.FirstName,
            Property.Colour,
            Property.Size
        );
    }

    public partial class Cat
    {
        _s _p => _s.define(
            Property.Colour,
            Property.Size,
            Computed.PrintSizeAndColour
        );
    }

    public void Execute()
    {
        var dog = new Dog() { Colour = "Red", Size = "Big" };
        PrintCat(dog);
    }

    public void PrintCat(Cat cat)
    {
        Console.WriteLine(cat.PrintSizeAndColour);
    }
}