// ReSharper disable InconsistentNaming

namespace Shpec;

public class _member
{
    public static readonly _member _default = new();
    public static implicit operator _member(string v) => _default;
    public static implicit operator string(_member p) => "";
    public static implicit operator _member(int v) => _default;
    public static implicit operator int(_member p) => 0;
}

public class _property : _member
{
    public _property() { }
}

public static class _property<T>
{
    public static T must(Func<T, bool> predicate) => default;
}

public class _computed : _member
{
    public _computed(object o) { }
}

public class _s
{
    public static _s define(params _member[] properties) => new();
}
public static class ValidationExtensions
{
    public static _member must<T>(this T _, Func<T, bool> predicate) => _member._default;
    public static _member match<T>(string regex) => _member._default;
}