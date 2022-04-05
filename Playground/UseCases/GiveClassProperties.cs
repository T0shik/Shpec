using Shpec;

namespace Playground.UseCases;

internal partial class GiveClassProperties : IUseCase
{
    _schema _p =>
        _schema.define(
            Property.FirstName,
            Property.LastName,
            Property.Age
        );

    public void Execute()
    {
        var a = new GiveClassProperties { FirstName = "foo", LastName = "bar", Age = 55 };
    }
}
