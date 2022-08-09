using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.Functions;

internal class IdentifierRewriter : CSharpSyntaxRewriter
{
    private readonly string _find;
    private readonly string _replace;

    internal IdentifierRewriter(
        string find,
        string replace
    )
    {
        _find = find;
        _replace = replace;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (node.Identifier.Text == _find)
        {
            return node.WithIdentifier(Identifier(_replace));
        }

        return node;
    }
}