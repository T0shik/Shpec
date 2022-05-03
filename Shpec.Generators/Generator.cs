using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;
using Shpec.Generators.Functions;
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

        var propertyDeclarations = syntaxReceiver.propertyDeclarations.Declarations;
        var computedProperties = syntaxReceiver.computedPropertyDeclarations.Declarations;

        var addImplicitConversions = new AddImplicitConversion(propertyDeclarations);
        var transformFactory = new TransformFactory(propertyDeclarations);
        var transformComputedPropertyExpression = transformFactory.TransformComputedPropertyExpression();

        var namespaceSeeds = syntaxReceiver.Declarations.Select(declaration => new NamespaceSeed(
            declaration.Namespace,
            new(
                declaration.Class.Identifier,
                declaration.Class.Accessibility,
                BuildParents(declaration.Class.Parent),
                declaration.Members.Select<string, Seed>(x =>
                    {
                        var propertyDefinition = propertyDeclarations.FirstOrDefault(d => d.Identifier == x);
                        if (propertyDefinition != null)
                        {
                            var (identifier, syntaxKind, validation) = propertyDefinition;

                            var validationSeed = MapValidation.Map(validation);

                            return new PropertySeed(identifier, syntaxKind, validationSeed);
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

                        throw new($"Failed to find declaration for {x} of {declaration.Class.Identifier}");
                    })
                    .ToList()
                    .AsReadOnly(),
                ImmutableArray<ConversionSeed>.Empty,
                declaration.Class.Static
            )
        )).ToList().AsReadOnly();

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
            Try(() => context.AddSource(classGen.SourceName, classGen.Source), $"generating source for {classGen.SourceName}");
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
                parent.Static
            );
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}