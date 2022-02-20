using Microsoft.CodeAnalysis;

namespace Shpec.Generators;

[Generator(LanguageNames.CSharp)]
public class TypeGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var syntaxReceiver = (GodGenerator)context.SyntaxReceiver;

        foreach (var declaration in syntaxReceiver.Declarations.Schemas)
        {
            var properties = declaration.Properties
                .Select(x => syntaxReceiver.Definitions.PropertyDefinitions
                    .FirstOrDefault(pd => pd.Identifier == x))
                .ToList();

            var classGen = new SchemaClassGenerator(
                declaration.NameSpace,
                declaration.ParentClass,
                declaration.Clazz,
                properties
            );

            context.AddSource($"{classGen.Name}.g", classGen.Source);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new GodGenerator());
    }
}