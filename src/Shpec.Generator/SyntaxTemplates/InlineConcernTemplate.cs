using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Functions;
using Shpec.Generator.Utils;

namespace Shpec.Generator.SyntaxTemplates;

// very infidel component, needs more thought
class InlineConcernTemplate
{
    public static IEnumerable<StatementSyntax> InlineSetterConcerns(ConcernSeed seed, string fieldIdentifier)
    {
        var parameterName = seed.Declaration.ParameterList.Parameters.First().Identifier.Text;
        if (seed.Type == FunctionType.Action)
        {
            var ir = new IdentifierRewriter(parameterName, "value");
            foreach (var statementSyntax in seed.Declaration.Body.Statements)
            {
                yield return (StatementSyntax)ir.Visit(statementSyntax);
            }
        }
        else if (seed.Type == FunctionType.Function)
        {
            throw new ShpecGenerationException("function concern feature not implemented yet.");
        }
    }
}