using Shpec;

namespace Playground.UseCases;

public partial class class_to_struct_and_back : IUseCase
{
    public partial class A
    {
        Properties _p => new(
            Property.FirstName,
            Property.LastName,
            Property.Age
        );
    }
    
    public partial struct B
    {
        Properties _p => new(
            Property.FirstName,
            Property.LastName
        );
    }
    
    public void Execute()
    {
        B b = new A() { FirstName = "A", LastName = "B", Age = 10 };
        Console.WriteLine($"A => B: f:{b.FirstName};l:{b.LastName};");
        A a = b;
        Console.WriteLine($"A => B => A: f:{a.FirstName};l:{a.LastName};a:{a.Age}");
    }
}
