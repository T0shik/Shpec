using System;

namespace Shpec;

public class Is
{
    public static readonly SpecProperty String = SpecProperty.Default;
    public static readonly SpecProperty Int = SpecProperty.Default;
}

public class Spec
{
    public static SpecProperty DefineProperty<T>()
    {
        return new SpecProperty();
    }
}

public struct SpecProperty
{
    public static readonly SpecProperty Default = new();
}

public struct Schema
{
    public static Schema Define(params SpecProperty[] properties)
    {
        return new Schema();
    }
}

[System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SchemaAttribute : Attribute
{
}

[System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PropertyDefinitionsAttribute : Attribute
{
}