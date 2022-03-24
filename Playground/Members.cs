using Shpec;

#pragma warning disable CS8618

namespace Playground;

public static class Property
{
    public static string FirstName => new _property();
    public static string LastName => new _property();
    public static int Age => new _property();
}

public class Computed
{
    public static _computed<string> FullName => _compute<string>.from($"{Property.FirstName} {Property.LastName}");
    public static _computed<string> Initials => _compute<string>.from($"{Property.FirstName[0]}.{Property.LastName[0]}.");
}
