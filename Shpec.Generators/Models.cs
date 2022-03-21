using Microsoft.CodeAnalysis.CSharp;

namespace Shpec.Generators;

public record PropertySeed(string Identifier, SyntaxKind ValueType);

public record ClassSeed(string Identifier)
{
    public readonly ClassSeed parent = null;
    public readonly ClassSeed[] classes = Array.Empty<ClassSeed>();
    public readonly PropertySeed[] properties = Array.Empty<PropertySeed>();
    public readonly bool partial = false;
}

public record NamespaceSeed(string Identifier, ClassSeed Class);

public class Node<T>
{
    public T Value { get; set; }
    public T Next { get; set; }
    public bool HasNext() => Next != null;
}

public record Definition;
public record PropertyDefinition(string Identifier, SyntaxKind Type) : Definition;
public record Declaration(string Namespace, string Class, IEnumerable<string> Properties);