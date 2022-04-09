using Shpec;

namespace Playground.UseCases;

public partial class convert_one_type_to_other : IUseCase
{
    public partial class PersonOne
    {
        _s _p => _s.define(
            Property.FirstName,
            Property.LastName,
            Property.Age
        );
    }

    public partial class PersonTwo
    {
        _s _p => _s.define(
            Property.FirstName,
            Property.LastName
            //Computed.FullName
        );
    }

    public void Execute()
    {
        PersonTwo two = new PersonOne() { FirstName = "First", LastName = "Last", Age = 10 };
        //Console.WriteLine(two.FullName);
    }
}