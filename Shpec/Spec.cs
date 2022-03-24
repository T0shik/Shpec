// ReSharper disable InconsistentNaming

namespace Shpec;

public class _member
{
    private static readonly _member _default = new();
    public static implicit operator _member(string v) => _default;
    public static implicit operator string(_member p) => "";
    public static implicit operator _member(int v) => _default;
    public static implicit operator int(_member p) => 0;
}

public class _property : _member{}

public class _computed<T> : _member{}

public class _schema
{
    public static _schema declare(params _member[] properties) => null;
}

public static class _compute<T>
{
    public static _computed<T> from(object o) => null;

    // todo: evaluate later, maybe discard
    // public static _computed<T> from<T1>(T1 o1, Func<T1, T> fn) => null;
    // public static _computed<T> from<T1, T2>(T1 o1, T2 o2, Func<T1, T2, T> fn) => null;
    // public static _computed<T> from<T1, T2, T3>(T1 o1, T2 o2, T3 o3, Func<T1, T2, T3, T> fn) => null;
}
