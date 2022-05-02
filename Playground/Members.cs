using Shpec;

#pragma warning disable CS8618

namespace Playground;

public static class Property
{
    public static string FirstName => new _property();
    public static string LastName => new _property();
    public static string Colour => new _property();
    public static string Size => new _property();
    public static int Age => new _property();
    public static int NumericIdentifier => _property<int>.must(x => x > 0);
}

public class Computed
{
    public static int BirthYear => new _computed(DateTime.UtcNow.Year - Property.Age);
    public static string FullName => new _computed($"{Property.FirstName} {Property.LastName}");
    public static string PrintSizeAndColour => new _computed($"Cat afd size: {Property.Size} and colour: {Property.Colour}");
    public static string Initials => new _computed(() =>
    {
        var fn = Property.FirstName[0];
        var ln = Property.LastName[0];
        return $"{fn}.{ln}.";
    });
}
