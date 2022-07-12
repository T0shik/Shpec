using Shpec;

namespace Playground.UseCases;

public partial class give_class_properties : IUseCase
{
    Members _p => new(
        Property.FirstName,
        Property.LastName,
        Property.Age
    );

    public void Execute()
    {
        var a = new give_class_properties { FirstName = "foo", LastName = "bar", Age = 55 };
    }
}
