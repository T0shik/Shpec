using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Functions;
using Shpec.Generator.SyntaxTemplates;
using Shpec.Generator.Utils;

namespace Shpec.Generator;

public class TranslationContext
{
    internal IEnumerable<NamespaceSeed> Translate(Main.ReadResult readResult)
    {
        var (properties, computedProperties, methods, usages) = readResult;
        var reWriteMembers = new MemberAccessRewriter(properties, computedProperties);
        var createConcern = new CreateConcern(methods);
        var addImplicitConversions = new AddImplicitConversion(properties);

        var namespaceSeeds = usages.Select(usage =>
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

        return namespaceSeeds
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
}