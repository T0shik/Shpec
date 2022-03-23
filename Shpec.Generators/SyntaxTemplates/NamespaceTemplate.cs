using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generators.SyntaxTemplates;

class NamespaceTemplate
{
    public static MemberDeclarationSyntax Create(NamespaceSeed seed)
    {
        return FileScopedNamespaceDeclaration(
                IdentifierName(seed.identifier))
            .WithMembers(
                SingletonList(ClassTemplate.Create(seed.clazz))
            );
    }
}