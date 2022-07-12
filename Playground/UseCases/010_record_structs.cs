using Shpec;
namespace Playground.UseCases;


public class record_structs : IUseCase
{
    public void Execute()
    {
        var r = new RecordStruct("Foo", "Bar");
        Console.WriteLine(r);
    }
}
public partial record struct RecordStruct
{
    private Members _p => new(
        Property.FirstName,
        Property.LastName
    );
}
