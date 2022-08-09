using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shpec.Generator.Functions;

public class CreateConcern
{
    private readonly Dictionary<string, MethodDeclarationSyntax> _methods;

    public CreateConcern(Dictionary<string, MethodDeclarationSyntax> methods)
    {
        _methods = methods;
    }

    internal IReadOnlyCollection<ConcernSeed> For(MemberUsage memberUsage)
    {
        return memberUsage
            .Concerns
            .Select(concern =>
            {
                var method = _methods[concern.Identifier];
                var type = method.ReturnType switch
                {
                    PredefinedTypeSyntax { Keyword.Text: "void" } => FunctionType.Action,
                    _ => FunctionType.Function,
                };
                return new ConcernSeed(method, concern.PointCut, type);
            })
            .ToArray();
    }
}