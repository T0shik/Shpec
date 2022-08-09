namespace Shpec.Generator;

public enum PointCut
{
    All,
    Before,
    After,
    Get,
    BeforeGet,
    AfterGet,
    Set,
    BeforeSet,
    AfterSet,
}

public enum FunctionType
{
    /// <summary>
    /// void consumer, void Method(T value) { ... }
    /// </summary>
    Action,
    
    /// <summary>
    /// with return type, T Method(T value) { ... }
    /// </summary>
    Function,
}
