using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Shpec.Generator.Functions;

internal class StatementTransformer
{
    private readonly TransformFactory _transformFactory;

    public StatementTransformer(TransformFactory transformFactory)
    {
        _transformFactory = transformFactory;
    }

    public StatementSyntax Transform(StatementSyntax st) =>
        st switch
        {
            BlockSyntax e => TransformBlockSyntax(e),
            LocalDeclarationStatementSyntax a => TransformLocalDeclarationStatementSyntax(a),
            ExpressionStatementSyntax e => TransformExpressionStatementSyntax(e),
            _ => st
        };

    private StatementSyntax TransformLocalDeclarationStatementSyntax(LocalDeclarationStatementSyntax st)
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
    
    /// <summary>
    /// Intended to transform expression statements
    ///
    ///    void Method()
    ///    {
    ///        // expression statement
    ///        var a = 5;
    ///    }
    /// 
    /// </summary>
    /// <param name="ess">expression statement</param>
    /// <returns>transformed expression statement</returns>
    private StatementSyntax TransformExpressionStatementSyntax(ExpressionStatementSyntax ess) =>
        ess.WithExpression(
            _transformFactory.PropertyExpressionTransformer().Transform(ess.Expression)
        );
    
    /// <summary>
    /// Intended to transform statements in the method body
    ///
    ///    void Method()
    ///    {
    ///        ...statements_to_transform 
    ///    }
    /// 
    /// </summary>
    /// <param name="bs">method body</param>
    /// <returns>transformed method body</returns>
    private StatementSyntax TransformBlockSyntax(BlockSyntax bs) =>
        bs.WithStatements(
            new(bs.Statements.Select(Transform))
        );
}