﻿using Shpec;

namespace Playground.UseCases;

public partial class nested_classes_support : IUseCase
{
    public void Execute()
    {
        new nested_classes_support().Do(new() { FirstName = "Foo", LastName = "Bar", Age = 10 });
    }

    public partial class Person
    {
        Members _p => new(
                Property.FirstName,
                Property.LastName,
                Property.Age
            );
    }

    public void Do(Person person)
    {
        Console.WriteLine(person.FirstName + " " + person.LastName + " " + person.Age);
    }
}
