using System.Linq;
using Microsoft.CodeAnalysis;
using Shpec.Generators.Generators;

namespace Shpec.Generators;

[Generator(LanguageNames.CSharp)]
public class SchemaGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not GodGenerator syntaxReceiver)
        {
            throw new ArgumentNullException(nameof(GodGenerator));
        }

        var definitions = syntaxReceiver.PropertyDefinitions.Definitions;
        foreach (var declaration in syntaxReceiver.Declarations)
        {
            NamespaceSeed ns = new(
                declaration.Namespace,
                new(declaration.Class)
                {
                    properties = declaration.Properties
                        .Select(x =>
                        {
                            var (identifier, syntaxKind) = definitions.First(d => d.Identifier == x);
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

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new GodGenerator());
    }
}