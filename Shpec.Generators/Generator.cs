using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;
using Shpec.Generators.Functions;

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

        var propertyDefinitions = syntaxReceiver.propertyDeclarations.Declarations;
        var computedProperties = syntaxReceiver.computedPropertyDeclarations.Declarations;
        
        var addImplicitConversions = new AddImplicitConversion(propertyDefinitions);
        var transformComputedPropertyExpression = new TransformComputedPropertyExpression(propertyDefinitions);

        var namespaceSeeds = syntaxReceiver.Declarations.Select(declaration => new NamespaceSeed(
            declaration.Namespace,
            new(
                declaration.Class.Identifier,
                declaration.Class.Accessibility,
                BuildParents(declaration.Class.Parent),
                declaration.Members.Select<string, Seed>(x =>
                    {
                        var propertyDefinition = propertyDefinitions.FirstOrDefault(d => d.Identifier == x);
                        if (propertyDefinition != null)
                        {
                            var (identifier, syntaxKind) = propertyDefinition;
                            return new PropertySeed(identifier, syntaxKind);
                        }

                        var computedDefinition = computedProperties.FirstOrDefault(d => d.Identifier == x);
                        if (computedDefinition != null)
                        {
                            var (identifier, syntaxKind, exp) = computedDefinition;
                            return new ComputedPropertySeed(
                                identifier,
                                syntaxKind,
                                transformComputedPropertyExpression.Transform(exp)
                            );
                        }

                        throw new($"Failed to find definition for {x} of {declaration.Class.Identifier}");
                    })
                    .ToList()
                    .AsReadOnly(),
                new(ArraySegment<ConversionSeed>.Empty),
                declaration.Class.Static
            )
        )).ToList().AsReadOnly();


        namespaceSeeds = namespaceSeeds.Select((ns, i) =>
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
        }).ToList().AsReadOnly();

        foreach (var classGen in namespaceSeeds.Select(x => new SchemaClassGenerator(x)))
        {
            context.AddSource(classGen.SourceName, classGen.Source);
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
                new(ArraySegment<Seed>.Empty),
                new(ArraySegment<ConversionSeed>.Empty),
                parent.Static
            );
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}