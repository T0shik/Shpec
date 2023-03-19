using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

class NamespaceTemplate
{
    public static MemberDeclarationSyntax Create(NamespaceSeed seed)
    {
        var body = ClassTemplate.Create(seed.Clazz);

        return FileScopedNamespaceDeclaration(IdentifierName(seed.Identifier))
            .WithMembers(SingletonList(body));
    }
}