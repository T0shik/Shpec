using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators;

record PropertyDefinition(string Identifier, SyntaxKind Type);

record ComputedPropertyDefinition(string Identifier, SyntaxKind Type, ExpressionSyntax Expression);

record Declaration(string Namespace, ClassDeclaration Class, IEnumerable<string> Members);

record ClassDeclaration(
    string Identifier,
    SyntaxKind Accessibility,
    ClassDeclaration? Parent,
    bool Static = false
);

record Seed;

record PropertySeed(string Identifier, SyntaxKind Type) : Seed;

record ComputedPropertySeed(string Identifier, SyntaxKind Type, ExpressionSyntax Expression) : Seed;

record ConversionSeed(NamespaceSeed Target, NamespaceSeed From, ReadOnlyCollection<PropertySeed> Properties);

record ClassSeed(string Identifier, SyntaxKind Accessibility, ClassSeed? Parent,
    ReadOnlyCollection<Seed> Members, ReadOnlyCollection<ConversionSeed> Conversions,
    bool Static) : Seed;

record NamespaceSeed(string Identifier, ClassSeed Clazz) : Seed;