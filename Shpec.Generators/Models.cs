using Microsoft.CodeAnalysis.CSharp;

namespace Shpec.Generators;

record PropertySeed(string Identifier, SyntaxKind Type);

record ClassSeed(string Identifier, SyntaxKind Accessibility, ClassSeed? Parent)
{
    public ClassSeed[] Classes { get; set; } = Array.Empty<ClassSeed>();
    public PropertySeed[] Properties { get; set; } = Array.Empty<PropertySeed>();
    public bool Partial { get; set; } = false;
}

record NamespaceSeed(string Identifier, ClassSeed Clazz);
record PropertyDefinition(string Identifier, SyntaxKind Type);

record Declaration(string Namespace, ClassDeclaration Class, IEnumerable<string> Properties);

record ClassDeclaration(string Identifier, SyntaxKind Accessibility, ClassDeclaration Parent);