using Shpec;

namespace Playground.UseCases;

public partial class method : IUseCase
{
    Members _p => new(
        Property.Age,
        Methods.IncrementAge
    );

    public void Execute()
    {
        var m = new method() {Age = 10};
        m.IncrementAge();

        if (m.Age != 11)
        {
            throw new($"failed to increment age: {m.Age}");
        }
    }
}