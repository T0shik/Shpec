using System.Linq;
using Microsoft.CodeAnalysis;
using Shpec.Generators.Generators;

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

        var definitions = syntaxReceiver.PropertyDefinitions.Definitions;
        foreach (var declaration in syntaxReceiver.Declarations)
        {
            NamespaceSeed ns = new(
                declaration.namespaze,
                new(
                    declaration.clazz.identifier,
                    declaration.clazz.accessibility,
                    BuildParents(declaration.clazz.parent)
                    )
                {
                    properties = declaration.properties
                        .Select(x =>
                        {
                            var (identifier, syntaxKind) = definitions.First(d => d.identifier == x);
                            return new PropertySeed(identifier, syntaxKind);
                        })
                        .ToArray(),
                    partial = true,
                }
            );
            
            var classGen = new SchemaClassGenerator(ns);
            context.AddSource(classGen.SourceName, classGen.Source);
        }
    }

    private static ClassSeed? BuildParents(ClassDeclataion parent)
    {
        return parent == null ? null : new(parent.identifier, parent.accessibility, BuildParents(parent.parent));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}