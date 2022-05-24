using Shpec;
namespace Playground.UseCases;


public partial class struct_ctor : IUseCase
{
    Properties _p => new(
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