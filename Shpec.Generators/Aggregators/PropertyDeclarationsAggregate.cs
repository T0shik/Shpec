using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Shpec.Generators.Utils;

namespace Shpec.Generators.Aggregators;

class PropertyDeclarationsAggregate : ISyntaxReceiver
{
    public List<PropertyDefinition> Declarations { get; set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Text: "_property" } } a)
        {
            AddSimple(a);
        }
        else if (syntaxNode is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax m } b)
        {
            if (m is { Expression: IdentifierNameSyntax { Identifier.Text: "Declare" }, Name: IdentifierNameSyntax { Identifier.Text: "_property" } })
            {
                AddSimple(b);
            }
            else
            {
                AddFrom(b);
            }
        }
    }

    private void AddSimple(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        var propertyDeclarationSyntax = invocationExpressionSyntax
            .GetParent<PropertyDeclarationSyntax>();

        var identifier = propertyDeclarationSyntax.Identifier.ToString();
        var type = propertyDeclarationSyntax.Type switch
        {
            PredefinedTypeSyntax a => a.Keyword.Text,
            IdentifierNameSyntax a => a.Identifier.Text,
            QualifiedNameSyntax a => a.ToString(),
            ArrayTypeSyntax a => a.ToString(),
            _ => throw new ShpecAggregationException("unsupported property declaration", propertyDeclarationSyntax),
        };

        bool HasArgument(string arg) => invocationExpressionSyntax.ArgumentList.Arguments.Any(x => x.NameColon.Name.Identifier.Text == arg);

        var immutable = HasArgument("immutable");

        Declarations.Add(new(identifier, type, ImmutableArray<AdHocValidation>.Empty, immutable));
    }

    private void AddFrom(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        if (!invocationExpressionSyntax.GetText().ToString().StartsWith("_property"))
        {
            return;
        }

        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax maes)
        {
            throw new ShpecAggregationException("Probably declared your property incorrectly(1)", invocationExpressionSyntax);
        }

        if (maes.Expression is not InvocationExpressionSyntax ies)
        {
            throw new ShpecAggregationException("Probably declared your property incorrectly(2)", invocationExpressionSyntax);
        }

        if (ies.Expression is not GenericNameSyntax gns)
        {
            throw new ShpecAggregationException("Probably declared your property incorrectly(3)", invocationExpressionSyntax);
        }

        TypeSyntax typeSyntax;
        try
        {
            typeSyntax = gns.TypeArgumentList.Arguments.Single();
        }
        catch
        {
            throw new Exception($"{gns.TypeArgumentList.Arguments}");
        }

        if (typeSyntax is not PredefinedTypeSyntax predefinedTypeSyntax)
        {
            throw new ShpecAggregationException("property is not pre-defined", invocationExpressionSyntax);
        }

        var validation = new List<BaseValidation>();
        var validationArgument = invocationExpressionSyntax.ArgumentList.Arguments.Single();
        if (validationArgument.Expression is SimpleLambdaExpressionSyntax sles)
        {
            validation.Add(new AdHocValidation(sles));
        }

        var propertyDeclarationSyntax = invocationExpressionSyntax
            .GetParent<PropertyDeclarationSyntax>();

        var identifier = propertyDeclarationSyntax.Identifier.ToString();
        var type = predefinedTypeSyntax.Keyword.Text;

        Declarations.Add(new(identifier, type, validation.AsReadOnly(), false));
    }
}