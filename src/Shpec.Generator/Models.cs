using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Utils;

namespace Shpec.Generator;

record BaseValidation;

record AdHocValidation(SimpleLambdaExpressionSyntax Exp) : BaseValidation;

record PropertyDefinition(
    string Identifier,
    TypeSyntax Type,
    IReadOnlyCollection<BaseValidation> Validation,
    bool Immutable
);

record ComputedPropertyDefinition(
    string Identifier,
    TypeSyntax Type,
    IReadOnlyCollection<BaseValidation> Validation,
    ExpressionSyntax Expression
);

record ConcernUsage(string Identifier, PointCut PointCut);

record MemberUsage(string Identifier, IReadOnlyCollection<ConcernUsage> Concerns);

record Usage(string Namespace, ClassDeclaration Class, IReadOnlyCollection<MemberUsage> Members);

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
    TypeSyntax Type,
    IReadOnlyCollection<ConcernSeed> Concerns,
    IReadOnlyCollection<ValidationSeed> Validations,
    bool Immutable
) : Seed;

record ComputedPropertySeed(
    string Identifier,
    TypeSyntax Type,
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
    bool Struct
) : Seed;

record NamespaceSeed(string Identifier, ClassSeed Clazz, IReadOnlyCollection<string> Usings) : Seed;