using Shpec;
using static Shpec.Declare;

#pragma warning disable CS8618

namespace Playground;

public static class Property
{
    public static string FirstName => _property();
    public static string LastName => _property();
    public static string Colour => _property();
    public static string Size => _property();
    public static int Age = _property();
}

public class Computed
{
    public static int BirthYear => _computed(DateTime.UtcNow.Year - Property.Age);
    public static string FullName => _computed($"{Property.FirstName} {Property.LastName}");
    public static string PrintSizeAndColour => _computed($"Cat afd size: {Property.Size} and colour: {Property.Colour}");
    public static string Initials => _computed(() =>
    {
        var fn = Property.FirstName[0];
        var ln = Property.LastName[0];
        return $"{fn}.{ln}.";
    });
}
