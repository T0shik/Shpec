using System;

namespace Shpec.Declare;

public class Member<T>
{
    public static Property<T> Property() => default;
    public static Computed<T> Computed(object o) => default;
}

public class Property<T> : Member<T>
{
    public Property<T> must(Func<T, bool> predicate) => this;
    public Property<T> match(string regex) => this;
    public static implicit operator T(Property<T> T) => default;
}

public class Computed<T> : Member<T>
{
    public Computed<T> must(Func<T, bool> predicate) => this;
    public Computed<T> match(string regex) => this;
    public static implicit operator T(Computed<T> T) => default;
}

public class MethodDefinitionAttribute : Attribute
{
}