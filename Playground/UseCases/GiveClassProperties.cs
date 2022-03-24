using Shpec;

namespace Playground.UseCases;

internal partial class GiveClassProperties : IUseCase
{
    [Schema] Schema _p = Schema.Define(
        GenericProperties.FirstName,
        GenericProperties.LastName,
        GenericProperties.Age
        );

    public void Execute()
    {
        var a = new GiveClassProperties
        {
            FirstName = "foo",
            LastName = "bar",
            Age = 55
        };
    }
}
