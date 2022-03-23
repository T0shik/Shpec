using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Shpec.Generators;

public record PropertySeed(string Identifier, SyntaxKind Type);

public record ClassSeed(string Identifier)
{
    public ClassSeed? parent = null;
    public ClassSeed[] classes = Array.Empty<ClassSeed>();
    public PropertySeed[] properties = Array.Empty<PropertySeed>();
    public bool partial = false;
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

public record Declaration(string Namespace, string Class, IEnumerable<string> Properties)
{
    public static Declaration operator +(Declaration a, Declaration b)
    {
        return new(a.Namespace, a.Class, a.Properties.Concat(b.Properties));
    }
}