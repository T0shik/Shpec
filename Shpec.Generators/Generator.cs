using Microsoft.CodeAnalysis;
using Shpec.Generators.Generators;
using Shpec.Generators.Transforms;

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

        var properties = syntaxReceiver.propertyDeclarations.Declarations;
        var computedProperties = syntaxReceiver.computedPropertyDeclarations.Declarations;
        foreach (var declaration in syntaxReceiver.Declarations)
        {
            NamespaceSeed ns = new(
                declaration.Namespace,
                new(
                    declaration.Class.Identifier,
                    declaration.Class.Accessibility,
                    BuildParents(declaration.Class.Parent)
                )
                {
                    Members = declaration.Members
                        .Select<string, Seed>(x =>
                        {
                            var propertyDefinition = properties.FirstOrDefault(d => d.Identifier == x);
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
                                    new TransformComputedPropertyExpression(properties).Transform(exp)
                                );
                            }

                            throw new Exception($"Failed to find definition for {x} of {declaration.Class.Identifier}");
                        })
                        .ToList(),
                    Static = declaration.Class.Static,
                }
            );

            var classGen = new SchemaClassGenerator(ns);
            context.AddSource(classGen.SourceName, classGen.Source);
        }
    }

    private static ClassSeed? BuildParents(ClassDeclaration? parent)
    {
        return parent == null
            ? null
            : new(parent.Identifier, parent.Accessibility, BuildParents(parent.Parent));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}
