using Shpec;
namespace Playground.UseCases;


public partial struct struct_ctor : IUseCase
{
    Members _p => new(
        Property.FirstName,
        Property.LastName,
        Property.Age
    );
    
    public void Execute()
    {
        var r = new struct_ctor("Foo", "Bar", 5);
        Console.WriteLine(r);
    }
}