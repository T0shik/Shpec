using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Functions;
using Shpec.Generator.SyntaxTemplates;
using Shpec.Generator.Utils;
using static Shpec.Generator.Utils.Ops;

namespace Shpec.Generator;

[Generator(LanguageNames.CSharp)]
public class Main : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
        {
            throw new ArgumentNullException(nameof(SyntaxReceiver));
        }

        var properties = syntaxReceiver.AggregatePropertyDefinitions.Captures;
        var computedProperties = syntaxReceiver.AggregateComputedPropertyDefinitions.Captures;
        var methods = syntaxReceiver.AggregateMethodDefinitions.Captures;

        var reWriteMembers = new MemberAccessRewriter(properties, computedProperties);
        var createConcern = new CreateConcern(methods);
        var addImplicitConversions = new AddImplicitConversion(properties);

        var namespaceSeeds = syntaxReceiver.Usages.Select(usage =>
        {
            IReadOnlyCollection<Seed> members = usage.Members.Select<MemberUsage, Seed>(x =>
                {
                    var propertyDefinition = properties.FirstOrDefault(d => d.Identifier == x.Identifier);
                    if (propertyDefinition != null)
                    {
                        var (identifier, syntaxKind, validation, immutable) = propertyDefinition;

                        return new PropertySeed(
                            identifier,
                            syntaxKind,
                            createConcern.For(x),
                            MapValidation.Map(validation),
                            immutable
                        );
                    }

                    var computedDefinition = computedProperties.FirstOrDefault(d => d.Identifier == x.Identifier);
                    if (computedDefinition != null)
                    {
                        var (identifier, syntaxKind, validation, exp) = computedDefinition;

                        return new ComputedPropertySeed(
                            identifier,
                            syntaxKind,
                            createConcern.For(x),
                            MapValidation.Map(validation),
                            (ExpressionSyntax)reWriteMembers.Visit(exp)
                        );
                    }

                    if (methods.ContainsKey(x.Identifier))
                    {
                        var block = (BlockSyntax?)reWriteMembers.Visit(methods[x.Identifier].Body);
                        if (block == null)
                        {
                            throw new ShpecGenerationException("failed to re-write block");
                        }

                        return new MethodSeed(
                            MethodTemplate.Transform(methods[x.Identifier], block),
                            createConcern.For(x)
                        );
                    }

                    throw new($"Failed to find declaration for {x} of {usage.Class.Identifier}");
                })
                .ToList()
                .AsReadOnly();

            ClassSeed classSeed = new(
                usage.Class.Identifier,
                usage.Class.Accessibility,
                BuildParents(usage.Class.Parent),
                members,
                ImmutableArray<ConversionSeed>.Empty,
                usage.Class.Static,
                usage.Class.Record,
                usage.Class.Struct
            );

            var usings = DetermineUsings.From(members);

            return new NamespaceSeed(usage.Namespace, classSeed, usings);
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