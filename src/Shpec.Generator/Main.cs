using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Aggregators;
using Shpec.Generator.Functions;
using Shpec.Generator.SyntaxTemplates;
using Shpec.Generator.Utils;
using static Shpec.Generator.Utils.Ops;

namespace Shpec.Generator;

[Generator(LanguageNames.CSharp)]
public class Main : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var readResult = Read(context);
        
        var translationResult = new TranslationContext(readResult)
            .Translate();
        
        new OutputTranslation(translationResult)
            .AddSourcesTo(context);
    }

    private static ReadResult Read(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
        {
            throw new ArgumentNullException(nameof(SyntaxReceiver));
        }

        return new(
            syntaxReceiver.AggregatePropertyDefinitions.Captures,
            syntaxReceiver.AggregateComputedPropertyDefinitions.Captures,
            syntaxReceiver.AggregateMethodDefinitions.Captures,
            syntaxReceiver.Usages
        );
    }

    internal record ReadResult(
        List<PropertyDefinition> Properties,
        List<ComputedPropertyDefinition> ComputedProperties,
        Dictionary<string, MethodDeclarationSyntax> Methods,
        List<Usage> Usages
    );

    class SyntaxReceiver : ISyntaxReceiver
    {
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            AggregatePropertyDefinitions.OnVisitSyntaxNode(syntaxNode);
            AggregateComputedPropertyDefinitions.OnVisitSyntaxNode(syntaxNode);
            AggregateMethodDefinitions.OnVisitSyntaxNode(syntaxNode);
            AggregateUsages.OnVisitSyntaxNode(syntaxNode);
        }

        public AggregatePropertyDefinitions AggregatePropertyDefinitions { get; } = new();
        public AggregateComputedPropertyDefinitions AggregateComputedPropertyDefinitions { get; } = new();
        public AggregateMethodDefinitions AggregateMethodDefinitions { get; } = new();
        public AggregateUsages AggregateUsages { get; } = new();
        public List<Usage> Usages => AggregateUsages.Captures.Values.ToList();
    }
}