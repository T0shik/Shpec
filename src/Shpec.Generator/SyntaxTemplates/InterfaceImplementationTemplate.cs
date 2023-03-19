using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

/// <summary>
/// covers generating ": One, Two" part of implementation in Example:
///     public class A : One, Two
///     {
///      ...
///     }
/// </summary>
class InterfaceImplementationTemplate
{
    internal static BaseListSyntax Create(ClassSeed seed)
    {
        var nodes = new List<SyntaxNodeOrToken>();
        foreach (var i in seed.Interfaces)
        {
            nodes.Add(SimpleBaseType(IdentifierName(i.Identifier)));
            nodes.Add(Token(SyntaxKind.CommaToken));
        }

        nodes.RemoveAt(nodes.Count - 1);

        return BaseList(SeparatedList<BaseTypeSyntax>(nodes));
    }
}