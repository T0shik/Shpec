using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shpec.Generator.DefaultConcerns;
using Shpec.Generator.Functions;
using Shpec.Generator.SyntaxTemplates;
using Shpec.Generator.Utils;

namespace Shpec.Generator;

class TranslationContext
{
    private readonly List<PropertyDefinition> _properties;
    private readonly List<ComputedPropertyDefinition> _computedProperties;
    private readonly Dictionary<string, MethodDeclarationSyntax> _methods;
    private readonly List<Usage> _usages;

    internal TranslationContext(Main.ReadResult readResult)
    {
        var (properties, computedProperties, methods, usages) = readResult;
        _properties = properties;
        _computedProperties = computedProperties;
        _methods = methods;
        _usages = usages;
    }

    internal IEnumerable<NamespaceSeed> Translate()
    {
        var addImplicitConversions = new AddImplicitConversion(_properties);

        var namespaceSeeds = _usages.Select(usage =>
        {
            ImmutableList<Seed> memberSeeds = GetMemberSeeds(usage);
            var classSeed = usage.Type switch
            {
                DefinitionType.Class => BuildClassType(usage, memberSeeds),
                DefinitionType.Role => BuildRoleType(usage, memberSeeds),
                _ => throw new ShpecTranslationException("Managed to capture unknown definition type.")
            };

            var usings = DetermineUsings.From(memberSeeds);

            return new NamespaceSeed(usage.Namespace, classSeed, usings);
        }).ToList();

        return namespaceSeeds
            .Select((ns, i) =>
            {
                for (var j = 0; j < namespaceSeeds.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    ns = addImplicitConversions.To(ns, namespaceSeeds[j]);
                }

                return ns;
            });
    }

    private ImmutableList<Seed> GetMemberSeeds(Usage usage)
    {
        var reWriteMembers = new MemberAccessRewriter(_properties, _computedProperties);
        var createConcern = new CreateConcern(_methods);

        return usage.Members.Select<MemberUsage, Seed>(x =>
            {
                var propertyDefinition = _properties.FirstOrDefault(d => d.Identifier == x.Identifier);
                if (propertyDefinition != null)
                {
                    var (identifier, syntaxKind, validation, immutable) = propertyDefinition;

                    return new PropertySeed(
                        identifier,
                        syntaxKind,
                        createConcern.For(x),
                        MapValidation.Map(validation),
                        immutable
                    );
                }

                var computedDefinition = _computedProperties.FirstOrDefault(d => d.Identifier == x.Identifier);
                if (computedDefinition != null)
                {
                    var (identifier, syntaxKind, validation, exp) = computedDefinition;

                    return new ComputedPropertySeed(
                        identifier,
                        syntaxKind,
                        createConcern.For(x),
                        MapValidation.Map(validation),
                        (ExpressionSyntax)reWriteMembers.Visit(exp)
                    );
                }

                if (_methods.ContainsKey(x.Identifier))
                {
                    var block = (BlockSyntax?)reWriteMembers.Visit(_methods[x.Identifier].Body);
                    if (block == null)
                    {
                        throw new ShpecGenerationException("failed to re-write block");
                    }

                    return new MethodSeed(
                        MethodTemplate.Transform(_methods[x.Identifier], block),
                        createConcern.For(x)
                    );
                }

                throw new($"Failed to find declaration for {x} of {usage.Class.Identifier}");
            })
            .ToImmutableList();
    }

    private ClassSeed BuildClassType(Usage usage, IReadOnlyCollection<Seed> members)
    {
        return new(
            usage.Class.Identifier,
            usage.Class.Accessibility,
            BuildParents(usage.Class.Parent),
            members,
            ImmutableArray<ConversionSeed>.Empty,
            usage.Class.Static,
            usage.Class.Record,
            usage.Class.Struct
        );

        static ClassSeed? BuildParents(ClassDeclaration? parent)
        {
            return parent == null
                ? null
                : new(
                    parent.Identifier,
                    parent.Accessibility,
                    BuildParents(parent.Parent),
                    ImmutableArray<Seed>.Empty,
                    ImmutableArray<ConversionSeed>.Empty,
                    parent.Static,
                    parent.Record,
                    parent.Struct
                );
        }
    }

    private ClassSeed BuildRoleType(Usage usage, ImmutableList<Seed> members)
    {
        if (usage.Class.Parent == null)
        {
            throw new ShpecTranslationException("Role has to be contained within a context.");
        }
        
        var finalMembers = members.AddRange(
            CreateRoleToContextMemberSeeds(usage.Class.Parent)
        );
        return new(
            usage.Class.Identifier,
            usage.Class.Accessibility,
            BuildParents(usage.Class, true),
            finalMembers,
            ImmutableArray<ConversionSeed>.Empty,
            usage.Class.Static,
            usage.Class.Record,
            usage.Class.Struct,
            StrictConversions: true
        );

        static ClassSeed? BuildParents(ClassDeclaration @this, bool directParent)
        {
            var parent = @this.Parent;
            return parent == null
                ? null
                : new ClassSeed(
                    parent.Identifier,
                    parent.Accessibility,
                    BuildParents(parent, false),
                    directParent ? CreateContextRoleProperties(@this).ToImmutableList() : ImmutableArray<Seed>.Empty,
                    ImmutableArray<ConversionSeed>.Empty,
                    parent.Static,
                    parent.Record,
                    parent.Struct
                );
        }

        static IEnumerable<Seed> CreateContextRoleProperties(ClassDeclaration theRole)
        {
            yield return new RolePropertySeed(
                theRole.Identifier.Replace("Role", ""),
                theRole.Identifier,
                Concerns: new[]
                {
                    DefaultRoleConcerns.AssignContextConcern(theRole)
                }
            );
        }

        static IEnumerable<Seed> CreateRoleToContextMemberSeeds(ClassDeclaration theContext)
        {
            yield return new PropertySeed(
                "Context",
                theContext.Identifier,
                ImmutableList<ConcernSeed>.Empty,
                ImmutableList<ValidationSeed>.Empty,
                false,
                IncludeInCtor: false
            );
        }
    }
}