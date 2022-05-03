﻿using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shpec.Generators.SyntaxTemplates;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators;

class SchemaClassGenerator
{
    private readonly NamespaceSeed _seed;

    internal SchemaClassGenerator(NamespaceSeed seed)
    {
        _seed = seed;
    }

    internal SourceText Source => CompilationUnit()
        .WithUsings(
                List<UsingDirectiveSyntax>(
                    new UsingDirectiveSyntax[]{
                        UsingDirective(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Collections")),
                                IdentifierName("Immutable"))),
                        UsingDirective(
                            IdentifierName("Shpec"))}))
        .WithMembers(SingletonList(NamespaceTemplate.Create(_seed)))
        .NormalizeWhitespace()
        .GetText(Encoding.UTF8);

    internal string SourceName => GetName();

    private string GetName()
    {
        var sb = new StringBuilder();
        sb.Append(_seed.Identifier);
        var i = sb.Length;
        var cc = _seed.Clazz;
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