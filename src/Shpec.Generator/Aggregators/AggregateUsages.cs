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
        if (syntaxNode is not PropertyDeclarationSyntax propertyDeclaration)
        {
            return;
        }

        if (propertyDeclaration.Type is not IdentifierNameSyntax { Identifier.Text: "Members" } propertyType)
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
            throw new ShpecAggregationException("failed to determine namespace", propertyDeclaration);
        }

        var ns = namespaceDeclaration.Name.ToString();

        var key = $"{ns}.{clazz}";
        if (Captures.ContainsKey(key))
        {
            // merge members with existing
            var d = Captures[key];
            Captures[key] = d with { Members = d.Members.Concat(members).ToArray() };
        }
        else
        {
            Captures[key] = new(ns, clazz, members.ToArray());
        }
    }

    private static TypeDeclaration ResolveClassHierarchy(SyntaxNode syntaxNode) => syntaxNode switch
    {
        ClassDeclarationSyntax a => CaptureClassHierarchy(a),
        InterfaceDeclarationSyntax a => CaptureClassHierarchy(a),
        StructDeclarationSyntax a => CaptureClassHierarchy(a, stract: true),
        RecordDeclarationSyntax { RawKind: (int)SyntaxKind.RecordDeclaration } a => CaptureClassHierarchy(a, record: true),
        RecordDeclarationSyntax { RawKind: (int)SyntaxKind.RecordStructDeclaration } a => CaptureClassHierarchy(a, record: true, stract: true),
        _ => throw new ShpecAggregationException("failed to ResolveClassHierarchy for enclosing type", syntaxNode),
    };

    private static TypeDeclaration CaptureClassHierarchy(TypeDeclarationSyntax typeDeclaration, bool record = false, bool stract = false)
    {
        var id = typeDeclaration.Identifier.ToString();
        var typeKey = typeDeclaration.Keyword.Text;

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
            return new(id, accessibility, null, typeKey, statik, record, stract);
        }

        return new(id, accessibility, ResolveClassHierarchy(parent), typeKey, statik, record, stract);
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
            _ => throw new ShpecAggregationException("unexpected members declaration", propertyDeclaration),
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

            // member with advice
            if (arg.Expression is TupleExpressionSyntax tes)
            {
                yield return BuildMemberWithAdvice(tes);
                continue;
            }

            throw new ShpecAggregationException("unexpected member usage declaration", propertyDeclaration);
        }

        MemberUsage BuildMemberWithAdvice(TupleExpressionSyntax tes)
        {
            List<ConcernUsage> concerns = new();
            var member = tes.Arguments[0];
            if (member.Expression is not IdentifierNameSyntax ins)
            {
                throw new ShpecAggregationException("bad advice application to property", tes);
            }

            var memberIdentifier = ins.Identifier.Text;
            for (var i = 1; i < tes.Arguments.Count; i++)
            {
                var arg = tes.Arguments[i];
                if (arg.Expression is not InvocationExpressionSyntax ies)
                {
                    throw new ShpecAggregationException("bad advice application syntax", tes);
                }

                var pointCut = GetPointCut(ies);
                var adviceIdentifier = GetAdviceIdentifier(ies);

                concerns.Add(new(adviceIdentifier, pointCut));
            }

            return new MemberUsage(memberIdentifier, concerns);

            PointCut GetPointCut(InvocationExpressionSyntax ies)
            {
                var pointCutString = ies.Expression switch
                {
                    MemberAccessExpressionSyntax x => x.Name.Identifier.Text,
                    IdentifierNameSyntax x => x.Identifier.Text,
                };

                if (Enum.TryParse<PointCut>(pointCutString, out var pointCut))
                {
                    return pointCut;
                }

                throw new ShpecAggregationException($"Invalid point cut \"{pointCutString}\"", tes);
            }

            string GetAdviceIdentifier(InvocationExpressionSyntax ies)
            {
                var arg = ies.ArgumentList.Arguments.First();

                return arg.Expression switch
                {
                    IdentifierNameSyntax x => x.Identifier.Text,
                    MemberAccessExpressionSyntax x => x.Name.Identifier.Text,
                    _ => throw new ShpecAggregationException("failed to get advice identifier", arg),
                };
            }
        }
    }
}