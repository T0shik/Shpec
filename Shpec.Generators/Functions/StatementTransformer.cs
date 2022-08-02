using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace Shpec.Generators.Functions;

internal class StatementTransformer
{
    private readonly TransformFactory _transformFactory;

    public StatementTransformer(TransformFactory transformFactory)
    {
        _transformFactory = transformFactory;
    }

    public StatementSyntax Transform(StatementSyntax st) => st switch
    {
        LocalDeclarationStatementSyntax a => Transform(a),
        _ => st
    };

    public StatementSyntax Transform(LocalDeclarationStatementSyntax st)
    {
        var transformer = _transformFactory.PropertyExpressionTransformer();
        var variables = st.Declaration.Variables
            .Select(v =>
            {
                var i = v.Initializer ?? throw new ArgumentNullException($"statement initializer is null, don't know what this means. {Environment.NewLine}{v.ToFullString()}");

                var vv = transformer.Transform(i.Value);
                return v.WithInitializer(
                        i.WithValue(vv)
                    );
            }).ToList();


        if (variables == null)
        {
            throw new Exception($"declaration variables are null after transform, don't know what this means. {Environment.NewLine}{st.Declaration.ToFullString()}");
        }

        return st.WithDeclaration(
                st.Declaration.WithVariables(
                    new SeparatedSyntaxList<VariableDeclaratorSyntax>().AddRange(variables)
                )
            );
    }
}
