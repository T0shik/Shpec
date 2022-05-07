using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generators;

record BaseValidation;
record AdHocValidation(SimpleLambdaExpressionSyntax Exp) : BaseValidation;
record PropertyDefinition(string Identifier, string Type, IReadOnlyCollection<BaseValidation> Validation);

record ComputedPropertyDefinition(string Identifier, string Type, IReadOnlyCollection<BaseValidation> Validation, ExpressionSyntax Expression);

record Declaration(string Namespace, ClassDeclaration Class, IEnumerable<string> Members);

record ClassDeclaration(
    string Identifier,
    SyntaxKind Accessibility,
    ClassDeclaration? Parent,
    bool Static = false
);


record Seed;
record ValidationSeed;
record AdHocValidationSeed(ExpressionSyntax Expression) : ValidationSeed;

record PropertySeed(string Identifier, string Type, IReadOnlyCollection<ValidationSeed> Validations) : Seed;

record ComputedPropertySeed(string Identifier, string Type, IReadOnlyCollection<ValidationSeed> Validations, ExpressionSyntax Expression)
    : PropertySeed(Identifier, Type, Validations);

record ConversionSeed(NamespaceSeed Target, NamespaceSeed From, IReadOnlyCollection<string> Properties);

record ClassSeed(string Identifier, SyntaxKind Accessibility, ClassSeed? Parent,
    IReadOnlyCollection<Seed> Members, IReadOnlyCollection<ConversionSeed> Conversions,
    bool Static) : Seed;

record NamespaceSeed(string Identifier, ClassSeed Clazz) : Seed;