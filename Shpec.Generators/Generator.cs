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
                declaration.Namespace,
                new(
                    declaration.Class.Identifier,
                    declaration.Class.Accessibility,
                    BuildParents(declaration.Class.Parent)
                    )
                {
                    Properties = declaration.Properties
                        .Select(x =>
                        {
                            var (identifier, syntaxKind) = definitions.First(d => d.Identifier == x);
                            return new PropertySeed(identifier, syntaxKind);
                        })
                        .ToArray(),
                    Partial = true,
                }
            );
            
            var classGen = new SchemaClassGenerator(ns);
            context.AddSource(classGen.SourceName, classGen.Source);
        }
    }

    private static ClassSeed? BuildParents(ClassDeclaration parent)
    {
        return parent == null ? null : new(parent.Identifier, parent.Accessibility, BuildParents(parent.Parent));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}