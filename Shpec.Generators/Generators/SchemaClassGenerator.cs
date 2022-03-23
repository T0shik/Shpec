using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Shpec.Generators.SyntaxTemplates;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.Generators;

public class SchemaClassGenerator
{
    private readonly NamespaceSeed _seed;

    public SchemaClassGenerator(NamespaceSeed seed)
    {
        _seed = seed;
    }

    public SourceText Source => CompilationUnit()
        .WithMembers(SingletonList(NamespaceTemplate.Create(_seed)))
        .NormalizeWhitespace()
        .GetText(Encoding.UTF8);

    public string SourceName => GetName();

    private string GetName()
    {
        var sb = new StringBuilder();
        sb.Append(_seed.Identifier);
        var i = sb.Length;
        var cc = _seed.Class;
        while (cc != null)
        {
            sb.Insert(i, cc.Identifier);
            cc = cc.parent;
        }

        sb.Append(".g");
        return sb.ToString();
    }
}