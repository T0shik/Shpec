using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generator;

record BaseValidation;

record AdHocValidation(SimpleLambdaExpressionSyntax Exp) : BaseValidation;

record PropertyDefinition(
    string Identifier,
    string Type,
    IReadOnlyCollection<BaseValidation> Validation,
    bool Immutable
);

record ComputedPropertyDefinition(
    string Identifier,
    string Type,
    IReadOnlyCollection<BaseValidation> Validation,
    ExpressionSyntax Expression
);

record ConcernUsage(string Identifier, PointCut PointCut);

record MemberUsage(string Identifier, IReadOnlyCollection<ConcernUsage> Concerns);

record Usage(string Namespace, ClassDeclaration Class, IReadOnlyCollection<MemberUsage> Members, DefinitionType Type);

public enum DefinitionType { Class, Role, Unknown }

record ClassDeclaration(
    string Identifier,
    SyntaxKind Accessibility,
    ClassDeclaration? Parent,
    bool Static,
    bool Record,
    bool Struct
);

record Seed;

record ValidationSeed;

record AdHocValidationSeed(ExpressionSyntax Expression) : ValidationSeed;

record ConcernSeed(
    MethodDeclarationSyntax Declaration,
    PointCut PointCut,
    FunctionType Type
);

record PropertySeed(
    string Identifier,
    string Type,
    IReadOnlyCollection<ConcernSeed> Concerns,
    IReadOnlyCollection<ValidationSeed> Validations,
    bool Immutable,
    bool IncludeInCtor = true
) : Seed;

record RolePropertySeed(
    string Identifier,
    string Type,
    IReadOnlyCollection<ConcernSeed> Concerns
) : PropertySeed(Identifier, Type, Concerns, ImmutableList<ValidationSeed>.Empty, true);

record ComputedPropertySeed(
    string Identifier,
    string Type,
    IReadOnlyCollection<ConcernSeed> Concerns,
    IReadOnlyCollection<ValidationSeed> Validations,
    ExpressionSyntax Expression
) : PropertySeed(Identifier, Type, Concerns, Validations, true);

record MethodSeed(
    MemberDeclarationSyntax Syntax,
    IReadOnlyCollection<ConcernSeed> Concerns
) : Seed;

record ConversionSeed(NamespaceSeed Target, NamespaceSeed From, IReadOnlyCollection<string> Properties);

record ClassSeed(
    string Identifier,
    SyntaxKind Accessibility,
    ClassSeed? Parent,
    IReadOnlyCollection<Seed> Members,
    IReadOnlyCollection<ConversionSeed> Conversions,
    bool Static,
    bool Record,
    bool Struct,
    // only generate conversions when properties match 1:1
    bool StrictConversions = false
) : Seed;

record NamespaceSeed(string Identifier, ClassSeed Clazz, IReadOnlyCollection<string> Usings) : Seed;