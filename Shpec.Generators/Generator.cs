using System.Collections.Immutable;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generators.Functions;
using Shpec.Generators.SyntaxTemplates;
using static Shpec.Generators.Utils.Ops;

namespace Shpec.Generators;

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
            IReadOnlyCollection<Seed> members = declaration.Members.Select<string, Seed>(x =>
                {
                    var propertyDefinition = propertyDeclarations.FirstOrDefault(d => d.Identifier == x);
                    if (propertyDefinition != null)
                    {
                        var (identifier, syntaxKind, validation, immutable) = propertyDefinition;

                        var validationSeed = MapValidation.Map(validation);

                        return new PropertySeed(identifier, syntaxKind, validationSeed, immutable);
                    }

                    var computedDefinition = computedProperties.FirstOrDefault(d => d.Identifier == x);
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

                    if (methods.ContainsKey(x))
                    {
                        var block = (BlockSyntax) statementTransformer.Transform(methods[x].Body);
                        
                        return new MethodSeed(
                            MethodTemplate.Transform(methods[x], block)
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
        }).ToList().AsReadOnly();

        namespaceSeeds = namespaceSeeds
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
            })
            .ToList()
            .AsReadOnly();


        foreach (var classGen in namespaceSeeds.Select(x => new SchemaClassGenerator(x)))
        {
            Try(() => context.AddSource(classGen.SourceName, classGen.Source()), $"generating source for {classGen.SourceName}");
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