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

public class _computed : _member{
    public _computed(object o)
    {

    }
}

public class _schema
{
    public static _schema define(params _member[] properties) => null;
}
