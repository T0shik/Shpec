// ReSharper disable InconsistentNaming

namespace Shpec;

public partial class Member
{
    public static readonly Member Default = new();
    public static implicit operator string(Member p) => "";
    public static implicit operator int(Member p) => 0;
}

public static class Declare
{
    public static Member _property() => Member.Default;
    public static Property<T> _property<T>() => new();
    public static Property<T> _property<T>(Func<T, bool> predicate) => new();
    public static Member _computed(object o) => new();
    public static Computed<T> _computed<T>(object o) => new();
}

public class Property<T> : Member
{
    public static T must(Func<T, bool> predicate) => default;
}

public class Computed<T> : Property<T> { }

public class Properties
{
    public Properties(params object[] members) { }
}

public static class ValidationExtensions
{
    public static Member must<T>(this T _, Func<T, bool> predicate) => Member.Default;
    public static Member match<T>(string regex) => Member.Default;
}