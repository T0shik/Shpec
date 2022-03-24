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
    public static string FullName => new _computed($"{Property.FirstName} {Property.LastName}");
    //public static string Introduce => new _computed(() => $"Hello, my name is {Property.FirstName} and I am {Property.Age} year's old.");
    //public static string Initials => new _computed(() =>
    //{
    //    var fn = Property.FirstName[0];
    //    var ln = Property.LastName[0];
    //    return $"{fn}.{ln}.";
    //});
}
