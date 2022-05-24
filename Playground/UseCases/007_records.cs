using Shpec;
namespace Playground.UseCases;


public class records : IUseCase
{
    public void Execute()
    {
        var r = new ExampleRecord("Foo", "Bar");
        Console.WriteLine(r);
    }
}
public partial record ExampleRecord
{
    private Properties _p => new(
        Property.FirstName,
        Property.LastName
    );
}
