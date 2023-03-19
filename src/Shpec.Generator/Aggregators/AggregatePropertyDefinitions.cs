using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Shpec.Generator.Utils;

namespace Shpec.Generator.Aggregators;

class AggregatePropertyDefinitions : ISyntaxReceiver
{
    public List<PropertyDefinition> Captures { get; set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is MemberAccessExpressionSyntax { Expression: GenericNameSyntax { Identifier.Text: "Member" }, Name.Identifier.Text: "Property" })
        {
            Add(syntaxNode.GetParent<InvocationExpressionSyntax>());
        }
    }

    private void Add(InvocationExpressionSyntax invocation)
    {
        var (identifier, typeSyntax) = GetDeclaration();

        var validationRules = GetValidationRules(invocation).ToImmutableList();
        Captures.Add(new(identifier, typeSyntax.ToString(), validationRules, false));

        (string Identifier, TypeSyntax Type) GetDeclaration()
        {
            var propertyDeclarationSyntax = invocation
                .TryGetParent<PropertyDeclarationSyntax>();

            if (propertyDeclarationSyntax != null)
            {
                return (propertyDeclarationSyntax.Identifier.ToString(), propertyDeclarationSyntax.Type);
            }

            var fieldDeclarationSyntax = invocation
                .TryGetParent<FieldDeclarationSyntax>();

            if (fieldDeclarationSyntax != null)
            {
                var identifier = fieldDeclarationSyntax.Declaration
                    .Variables
                    .Single()
                    .Identifier
                    .ToString();

                return (identifier, fieldDeclarationSyntax.Declaration.Type);
            }

            throw new ShpecAggregationException("unsupported property declaration", invocation.GetParent<MemberDeclarationSyntax>());
        }
    }

    static IEnumerable<BaseValidation> GetValidationRules(InvocationExpressionSyntax invocation)
    {
        if (invocation?.Parent is MemberAccessExpressionSyntax { Name.Identifier.Text: "must" } ma)
        {
            if (ma.Parent is InvocationExpressionSyntax pi)
            {
                var validationArgument = pi.ArgumentList.Arguments.Single();
                if (validationArgument.Expression is not SimpleLambdaExpressionSyntax sles)
                {
                    throw new ShpecAggregationException("unrecognized validation expression", invocation);
                }

                yield return new AdHocValidation(sles);
                foreach (var vr in GetValidationRules(pi))
                {
                    yield return vr;
                }
            }
            else
            {
                throw new ShpecAggregationException("unrecognized validation chain", invocation);
            }
        }
    }
}