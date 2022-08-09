using Shpec.Declare;

#pragma warning disable CS8618
namespace Playground;

public static class Property
{
    public static string FirstName => Member<string>.Property();
    public static string LastName => Member<string>.Property();
    public static string Colour => Member<string>.Property();
    public static string Size => Member<string>.Property();
    public static int Age = Member<int>.Property();
}

public class Computed
{
    public static int BirthYear =>  Member<int>.Computed(DateTime.UtcNow.Year - Property.Age);
    public static string FullName => Member<string>.Computed($"{Property.FirstName} {Property.LastName}");
    public static string PrintSizeAndColour => Member<string>.Computed($"size: {Property.Size} and colour: {Property.Colour}");
    public static string Initials => Member<string>.Computed(() =>
    {
        var fn = Property.FirstName[0];
        var ln = Property.LastName[0];
        return $"{fn}.{ln}.";
    });
}

public class Methods
{
    [MethodDefinition]
    public static void IncrementAge()
    {
        Property.Age += 1;
    }
}
