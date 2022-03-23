using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Shpec.Generators.SyntaxTemplates;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.Generators;

class SchemaClassGenerator
{
    private readonly NamespaceSeed _seed;

    internal SchemaClassGenerator(NamespaceSeed seed)
    {
        _seed = seed;
    }

    internal SourceText Source => CompilationUnit()
        .WithMembers(SingletonList(NamespaceTemplate.Create(_seed)))
        .NormalizeWhitespace()
        .GetText(Encoding.UTF8);

    internal string SourceName => GetName();

    private string GetName()
    {
        var sb = new StringBuilder();
        sb.Append(_seed.identifier);
        var i = sb.Length;
        var cc = _seed.clazz;
        while (cc != null)
        {
            sb.Insert(i, cc.identifier);
            cc = cc.parent;
        }

        sb.Append(".g");
        return sb.ToString();
    }
}