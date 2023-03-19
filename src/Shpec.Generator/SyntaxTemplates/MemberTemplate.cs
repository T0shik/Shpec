using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Utils;

namespace Shpec.Generator.SyntaxTemplates;

static class MemberTemplate
{
    internal static IEnumerable<MemberDeclarationSyntax> Create(Seed seed) => seed switch
    {
        ComputedPropertySeed cps => ComputedPropertyTemplate.Create(cps),
        PropertySeed ps => PropertyTemplate.Create(ps),
        ClassSeed cs => new[] { ClassTemplate.Create(cs) },
        MethodSeed ms => new[] { ms.Syntax },
        _ => throw new ShpecGenerationException($"Unhandled Seed in ClassTemplate.CreateClassDeclaration. Seed: {seed}"),
    };
}