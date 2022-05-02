using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators;

record BaseValidation;
record AdHocValidation(SimpleLambdaExpressionSyntax exp) : BaseValidation;
record PropertyDefinition(string Identifier, SyntaxKind Type, IReadOnlyCollection<BaseValidation> Validation);

record ComputedPropertyDefinition(string Identifier, SyntaxKind Type, IReadOnlyCollection<BaseValidation> Validation, ExpressionSyntax Expression);

record Declaration(string Namespace, ClassDeclaration Class, IEnumerable<string> Members);

record ClassDeclaration(
    string Identifier,
    SyntaxKind Accessibility,
    ClassDeclaration? Parent,
    bool Static = false
);


record Seed;
record ValidationSeed;
record KnownValidation(string identifier) : ValidationSeed;
record AdHocValidationSeed(ExpressionSyntax exp) : ValidationSeed;

record PropertySeed(string Identifier, SyntaxKind Type, IReadOnlyCollection<ValidationSeed> Validations) : Seed;

record ComputedPropertySeed(string Identifier, SyntaxKind Type, IReadOnlyCollection<ValidationSeed> Validations, ExpressionSyntax Expression)
    : PropertySeed(Identifier, Type, Validations);

record ConversionSeed(NamespaceSeed Target, NamespaceSeed From, IReadOnlyCollection<string> Properties);

record ClassSeed(string Identifier, SyntaxKind Accessibility, ClassSeed? Parent,
    IReadOnlyCollection<Seed> Members, IReadOnlyCollection<ConversionSeed> Conversions,
    bool Static) : Seed;

record NamespaceSeed(string Identifier, ClassSeed Clazz) : Seed;