using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.Utils;

namespace Shpec.Generator.Aggregators;

class AggregateUsages : ISyntaxReceiver
{
    public readonly Dictionary<string, Usage> Captures = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not PropertyDeclarationSyntax { Type: IdentifierNameSyntax { Identifier.Text: "Members" } } propertyDeclaration)
        {
            return;
        }

        var clazz = ResolveClassHierarchy(propertyDeclaration.Parent);
        var members = GetMembers(propertyDeclaration);
        BaseNamespaceDeclarationSyntax? namespaceDeclaration = propertyDeclaration.TryGetParent<FileScopedNamespaceDeclarationSyntax>();

        if (namespaceDeclaration == null)
        {
            namespaceDeclaration = propertyDeclaration.TryGetParent<NamespaceDeclarationSyntax>();
        }

        if (namespaceDeclaration == null)
        {
            throw new ShpecAggregationException("failed to determine namespace", syntaxNode);
        }

        var ns = namespaceDeclaration.Name.ToString();

        var key = $"{ns}.{clazz}";
        if (Captures.ContainsKey(key))
        {
            var d = Captures[key];
            Captures[key] = d with { Members = d.Members.Concat(members).ToArray() };
        }
        else
        {
            Captures[key] = new(ns, clazz, members.ToArray());
        }
    }

    private static ClassDeclaration ResolveClassHierarchy(SyntaxNode syntaxNode) => syntaxNode switch
    {
        ClassDeclarationSyntax a => CaptureClassHierarchy(a),
        StructDeclarationSyntax a => CaptureClassHierarchy(a, stract: true),
        RecordDeclarationSyntax { RawKind: (int)SyntaxKind.RecordDeclaration } a => CaptureClassHierarchy(a, record: true),
        RecordDeclarationSyntax { RawKind: (int)SyntaxKind.RecordStructDeclaration } a => CaptureClassHierarchy(a, record: true, stract: true),
        _ => throw new ShpecAggregationException("failed to ResolveClassHierarchy for enclosing type", syntaxNode),
    };

    private static ClassDeclaration CaptureClassHierarchy(TypeDeclarationSyntax typeDeclaration, bool record = false, bool stract = false)
    {
        var id = typeDeclaration.Identifier.ToString();

        var accessibility = typeDeclaration.Modifiers.First().ValueText switch
        {
            "public" => SyntaxKind.PublicKeyword,
            "private" => SyntaxKind.PrivateKeyword,
            "internal" => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.InternalKeyword
        };

        var statik = typeDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));

        var parent = typeDeclaration.TryGetParent<TypeDeclarationSyntax>();
        if (parent == null)
        {
            return new(id, accessibility, null, statik, record, stract);
        }

        return new(id, accessibility, ResolveClassHierarchy(parent), statik, record, stract);
    }

    private IEnumerable<MemberUsage> GetMembers(PropertyDeclarationSyntax propertyDeclaration)
    {
        if (propertyDeclaration.ExpressionBody == null)
        {
            throw new ShpecAggregationException("No expression body for members, use `=>` instead of `=`.", propertyDeclaration);
        }

        BaseObjectCreationExpressionSyntax exp = propertyDeclaration.ExpressionBody.Expression switch
        {
            ObjectCreationExpressionSyntax x => x,
            ImplicitObjectCreationExpressionSyntax x => x,
            _ => throw new ShpecAggregationException("unexpected members declaration.", propertyDeclaration),
        };

        foreach (var arg in exp.ArgumentList.Arguments)
        {
            var withStaticImport = arg.Expression is IdentifierNameSyntax;
            if (withStaticImport)
            {
                yield return new(arg.ToString().LastAccessor(), Array.Empty<ConcernUsage>());
                continue;
            }

            var fullReference = arg.Expression is MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax };
            if (fullReference)
            {
                yield return new(arg.ToString().LastAccessor(), Array.Empty<ConcernUsage>());
                continue;
            }

            var withConcern = arg.Expression as InvocationExpressionSyntax;
            if (withConcern != null)
            {
                yield return BuildMemberWithConcerns(withConcern);
                continue;
            }

            throw new ShpecAggregationException("unexpected member usage declaration", propertyDeclaration);
        }

        MemberUsage BuildMemberWithConcerns(InvocationExpressionSyntax rootInvocation)
        {
            var ies = rootInvocation;
            List<ConcernUsage> concerns = new();
            while (true)
            {
                var identifier = ies.ArgumentList.Arguments.First().Expression.ToString().LastAccessor();
                var mae = ies.Expression as MemberAccessExpressionSyntax;

                if (mae.Expression is InvocationExpressionSyntax next)
                {
                    var pointCutString = mae.Name.Identifier.Text.LastAccessor();

                    if (!Enum.TryParse<PointCut>(pointCutString, out var pointCut))
                    {
                        throw new ShpecAggregationException($"Invalid point cut {pointCutString}", rootInvocation);
                    }

                    concerns.Add(new(identifier, pointCut));
                    ies = next;
                    continue;
                }

                if (mae.Expression is IdentifierNameSyntax)
                {
                    return new MemberUsage(identifier, concerns);
                }

                throw new ShpecAggregationException("unexpected concern chain", rootInvocation);
            }
        }
    }
}