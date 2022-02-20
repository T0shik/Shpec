using Shpec;

namespace Playground;

[SpecDefinition]
public class GenericProperties
{
    public static readonly SpecProperty FirstName = Is.String;
    public static readonly SpecProperty LastName = Is.String;
    public static readonly SpecProperty Age = Is.Int;
}