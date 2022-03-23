using Shpec;

namespace Playground;

internal partial class GiveClassProperties
{
    [Schema] Schema _p = Schema.Define(
        GenericProperties.FirstName,
        GenericProperties.LastName,
        GenericProperties.Age
        );

    public GiveClassProperties Create()
    {
        return new GiveClassProperties
        {
            FirstName = "foo",
            LastName = "bar",
            Age = 55
        };
    }
}
