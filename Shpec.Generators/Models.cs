using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Shpec.Generators;

record PropertySeed(string identifier, SyntaxKind type);

record ClassSeed(string identifier, SyntaxKind accessibility, ClassSeed? parent)
{
    public ClassSeed[] classes { get; set; } = Array.Empty<ClassSeed>();
    public PropertySeed[] properties { get; set; } = Array.Empty<PropertySeed>();
    public bool partial { get; set; } = false;
}

record NamespaceSeed(string identifier, ClassSeed clazz);
record PropertyDefinition(string identifier, SyntaxKind type);

record Declaration(string namespaze, ClassDeclataion clazz, IEnumerable<string> properties);

record ClassDeclataion(string identifier, SyntaxKind accessibility, ClassDeclataion parent);