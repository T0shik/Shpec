using Shpec;
namespace Playground.UseCases;


public class structs : IUseCase
{
    public void Execute()
    {
        var s = new ExampleStruct{FirstName = "Foo", LastName = "Bar"};
        Console.WriteLine(s);
    }
}
public partial struct ExampleStruct
{
    Properties _p => new(
        Property.FirstName,
        Property.LastName
    );
}
