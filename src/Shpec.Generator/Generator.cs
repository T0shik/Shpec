using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Functions;
using Shpec.Generator.SyntaxTemplates;
using Shpec.Generator.Utils;
using static Shpec.Generator.Utils.Ops;

namespace Shpec.Generator;

[Generator(LanguageNames.CSharp)]
public class SchemaGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
        {
            throw new ArgumentNullException(nameof(SyntaxReceiver));
        }

        var propertyDeclarations = syntaxReceiver.AggregatePropertyDefinitions.Captures;
        var computedProperties = syntaxReceiver.AggregateComputedPropertyDefinitions.Captures;
        var methods = syntaxReceiver.AggregateMethodDefinitions.Captures;

        var addImplicitConversions = new AddImplicitConversion(propertyDeclarations);
        var transformFactory = new TransformFactory(propertyDeclarations, computedProperties);
        var transformComputedPropertyExpression = transformFactory.PropertyExpressionTransformer();
        var statementTransformer = transformFactory.StatementTransformer();

        var namespaceSeeds = syntaxReceiver.Usages.Select(declaration =>
        {
            IReadOnlyCollection<Seed> members = declaration.Members.Select<MemberUsage, Seed>(x =>
                {
                    var propertyDefinition = propertyDeclarations.FirstOrDefault(d => d.Identifier == x.Identifier);
                    if (propertyDefinition != null)
                    {
                        var (identifier, syntaxKind, validation, immutable) = propertyDefinition;

                        var validationSeed = MapValidation.Map(validation);

                        return new PropertySeed(identifier, syntaxKind, validationSeed, immutable);
                    }

                    var computedDefinition = computedProperties.FirstOrDefault(d => d.Identifier == x.Identifier);
                    if (computedDefinition != null)
                    {
                        var (identifier, syntaxKind, validation, exp) = computedDefinition;

                        var validationSeed = MapValidation.Map(validation);

                        return new ComputedPropertySeed(
                            identifier,
                            syntaxKind,
                            validationSeed,
                            transformComputedPropertyExpression.Transform(exp)
                        );
                    }

                    if (methods.ContainsKey(x.Identifier))
                    {
                        var block = (BlockSyntax)statementTransformer.Transform(methods[x.Identifier].Body);

                        return new MethodSeed(
                            MethodTemplate.Transform(methods[x.Identifier], block)
                        );
                    }

                    throw new($"Failed to find declaration for {x} of {declaration.Class.Identifier}");
                })
                .ToList()
                .AsReadOnly();

            ClassSeed classSeed = new(
                declaration.Class.Identifier,
                declaration.Class.Accessibility,
                BuildParents(declaration.Class.Parent),
                members,
                ImmutableArray<ConversionSeed>.Empty,
                declaration.Class.Static,
                declaration.Class.Record,
                declaration.Class.Struct
            );

            var usings = DetermineUsings.From(members);

            return new NamespaceSeed(declaration.Namespace, classSeed, usings);
        }).ToList();

        var enrichedNS = namespaceSeeds
            .Select((ns, i) =>
            {
                for (var j = 0; j < namespaceSeeds.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    ns = addImplicitConversions.To(ns, namespaceSeeds[j]);
                }

                return ns;
            });

        foreach (var ns in enrichedNS)
        {
            Try("generate sources", () =>
            {
                var (name, source) = SchemaClassGenerator.Generate(ns);
                context.AddSource(name, source);
            });
        }
    }

    private static ClassSeed? BuildParents(ClassDeclaration? parent)
    {
        return parent == null
            ? null
            : new(
                parent.Identifier,
                parent.Accessibility,
                BuildParents(parent.Parent),
                ImmutableArray<Seed>.Empty,
                ImmutableArray<ConversionSeed>.Empty,
                parent.Static,
                parent.Record,
                parent.Struct
            );
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}