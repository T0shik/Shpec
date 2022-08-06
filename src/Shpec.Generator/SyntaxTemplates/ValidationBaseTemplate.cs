using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Shpec.Generator.SyntaxTemplates;

public class ValidationBaseTemplate
{
    public static ClassDeclarationSyntax AddTo(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.WithBaseList(
            BaseList(
                SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(
                        IdentifierName("Shpec.Validation.IValidatable")))));
    }
}