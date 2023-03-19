using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shpec.Generator.SyntaxTemplates;
using Shpec.Generator.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator;

class OutputTranslation
{
    private readonly IEnumerable<NamespaceSeed> _seeds;

    public OutputTranslation(IEnumerable<NamespaceSeed> seeds)
    {
        _seeds = seeds;
    }


    public void AddSourcesTo(GeneratorExecutionContext context)
    {
        foreach (var ns in _seeds)
        {
            Ops.Try("generate sources", () =>
            {
                var (name, source) = Generate(ns);
                context.AddSource(name, source);
            });
        }
    }

    private static (string Name, SourceText Source) Generate(NamespaceSeed seed)
    {
        return (GetName(seed), GetSource(seed));
    }

    private static SourceText GetSource(NamespaceSeed seed)
    {
        List<UsingDirectiveSyntax> usings = new()
        {
            UsingDirective(
                QualifiedName(
                    QualifiedName(
                        IdentifierName("System"),
                        IdentifierName("Collections")
                    ),
                    IdentifierName("Generic")
                )
            ),

            UsingDirective(IdentifierName("Shpec")),

            UsingDirective(
                QualifiedName(
                    IdentifierName("Shpec"),
                    IdentifierName("Validation")
                )),
        };

        foreach (var usingSeed in seed.Usings)
        {
            usings.Add(UsingDirective(IdentifierName(usingSeed)));
        }


        return CompilationUnit()
            .WithUsings(List(usings))
            .WithMembers(SingletonList(NamespaceTemplate.Create(seed)))
            .NormalizeWhitespace()
            .GetText(Encoding.UTF8);
    }

    private static string GetName(NamespaceSeed seed)
    {
        var sb = new StringBuilder();
        sb.Append(seed.Identifier);
        var i = sb.Length;
        var cc = seed.Clazz;
        while (cc != null)
        {
            sb.Insert(i, '.');
            sb.Insert(i + 1, cc.Identifier);
            cc = cc.Parent;
        }

        sb.Append(".g");
        return sb.ToString();
    }
}