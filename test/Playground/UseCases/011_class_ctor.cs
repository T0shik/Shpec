using Shpec;
namespace Playground.UseCases;


public partial class class_ctor : IUseCase
{
    Members _p => new(
        Property.FirstName,
        Property.LastName,
        Property.Age
    );
    
    public void Execute()
    {
        var r = new class_ctor("Foo", "Bar", 5);
        Console.WriteLine(r);
    }
}