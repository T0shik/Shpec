// ReSharper disable InconsistentNaming

using System;
using System.Numerics;

namespace Shpec;

// todo: move classes in to separate namespaces for better intellisense
public class Member
{
    internal static readonly Member Default = new();
}

public class Member<T> : Member
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

public class Members
{
    public Members(params object[] members)
    {
    }
}