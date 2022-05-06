using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

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
        else if (syntaxNode is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax } b)
        {
            AddFrom(b);
        }
    }

    private void AddSimple(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        var propertyDeclarationSyntax = invocationExpressionSyntax
            .GetParent<PropertyDeclarationSyntax>();

        var identifier = propertyDeclarationSyntax.Identifier.ToString();
        if (propertyDeclarationSyntax.Type is not PredefinedTypeSyntax predefinedTypeSyntax)
        {
            throw new Exception($"Error:\n{propertyDeclarationSyntax.GetText()}");
        }

        var type = predefinedTypeSyntax.Keyword.Kind();

        Declarations.Add(new PropertyDefinition(identifier, type, ImmutableArray<AdHocValidation>.Empty));
    }

    private void AddFrom(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        if (!invocationExpressionSyntax.GetText().ToString().StartsWith("_property"))
        {
            return;
        }

        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax maes)
        {
            throw new Exception("bastard");
        }

        if (maes.Expression is not InvocationExpressionSyntax ies)
        {
            throw new Exception("bastard 2");
        }

        if (ies.Expression is not GenericNameSyntax gns)
        {
            throw new Exception("bastard 3");
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
            throw new Exception($"Error:\n{typeSyntax.GetText()}");
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
        var typeKind = predefinedTypeSyntax.Keyword.Kind();

        Declarations.Add(new PropertyDefinition(identifier, typeKind, validation.AsReadOnly()));
    }
}