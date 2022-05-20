﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class NamespaceTemplate
{
    public static MemberDeclarationSyntax Create(NamespaceSeed seed)
    {
        var classTemplate = ClassTemplate.Create(seed.Clazz);

        return FileScopedNamespaceDeclaration(IdentifierName(seed.Identifier))
            .WithMembers(SingletonList(classTemplate));
    }
}