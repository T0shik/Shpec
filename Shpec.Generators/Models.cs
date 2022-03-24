using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators;

record PropertyDefinition(string Identifier, SyntaxKind Type);
record ComputedPropertyDefinition(string Identifier, SyntaxKind Type, ExpressionSyntax Expression, bool Lambda);

record Declaration(string Namespace, ClassDeclaration Class, IEnumerable<string> Members);
record ClassDeclaration(
    string Identifier,
    SyntaxKind Accessibility,
    ClassDeclaration? Parent,
    bool Static = false
    );

record Seed;
record PropertySeed(string Identifier, SyntaxKind Type) : Seed;
record ComputedPropertySeed(string Identifier, SyntaxKind Type, ExpressionSyntax Expression, bool Lambda) : Seed;

record ClassSeed(string Identifier, SyntaxKind Accessibility, ClassSeed? Parent) : Seed
{
    public List<Seed> Members { get; set; } = new List<Seed>();
    public bool Static { get; set; } = false;
}

record NamespaceSeed(string Identifier, ClassSeed Clazz) : Seed;
