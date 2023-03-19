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
        IReadOnlyList<NamespaceSeed> namespaceSeeds = _usages.Select(usage =>
        {
            ImmutableList<Seed> memberSeeds = GetMemberSeeds(usage);
            var classSeed = usage.Type.Type switch
            {
                "class" or "record" or "struct" or "record struct" => BuildClassType(usage, memberSeeds),
                "interface" => BuildRoleType(usage, memberSeeds),
                _ => throw new ShpecTranslationException($"Managed to capture unknown definition type, {usage.Type.Type}")
            };

            var usings = DetermineUsings.From(memberSeeds);

            return new NamespaceSeed(usage.Namespace, classSeed, usings);
        }).ToList();

        namespaceSeeds = AddImplicitConversions(namespaceSeeds);
        namespaceSeeds = AddRoles(namespaceSeeds);

        return namespaceSeeds;
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

                throw new($"Failed to find declaration for {x} of {usage.Type.Identifier}");
            })
            .ToImmutableList();
    }

    private TypeSeed BuildClassType(Usage usage, IReadOnlyCollection<Seed> members)
    {
        return new ClassSeed(
            usage.Type.Identifier,
            usage.Type.Accessibility,
            BuildParents(usage.Type.Parent),
            members,
            ImmutableList<ConversionSeed>.Empty,
            ImmutableList<InterfaceSeed>.Empty,
            usage.Type.Static,
            usage.Type.Record,
            usage.Type.Struct
        );

        static ClassSeed? BuildParents(TypeDeclaration? parent)
        {
            return parent == null
                ? null
                : new(
                    parent.Identifier,
                    parent.Accessibility,
                    BuildParents(parent.Parent),
                    ImmutableArray<Seed>.Empty,
                    ImmutableList<ConversionSeed>.Empty,
                    ImmutableList<InterfaceSeed>.Empty,
                    parent.Static,
                    parent.Record,
                    parent.Struct,
                    CtorByDefault: false
                );
        }
    }

    private TypeSeed BuildRoleType(Usage usage, ImmutableList<Seed> members)
    {
        if (usage.Type.Parent == null)
        {
            throw new ShpecTranslationException("Role has to be contained within a context.");
        }

        var parent = BuildParent(usage.Type)!;
        var role = new RoleSeed(usage.Type.Identifier, parent, members);

        return role with
        {
            Parent = parent with
            {
                Members = CreateContextRoleProperties(usage, role).ToImmutableList()
            },
            Members = role.Members.AddRange(
                CreateRoleToContextMemberSeeds(usage, parent)
            )
        };

        static ClassSeed? BuildParent(TypeDeclaration @this)
        {
            var parent_ = @this.Parent;
            return parent_ == null
                ? null
                : new ClassSeed(
                    parent_.Identifier,
                    parent_.Accessibility,
                    BuildParent(parent_),
                    ImmutableArray<Seed>.Empty,
                    ImmutableList<ConversionSeed>.Empty,
                    ImmutableList<InterfaceSeed>.Empty,
                    parent_.Static,
                    parent_.Record,
                    parent_.Struct,
                    CtorByDefault: false
                );
        }

        static IEnumerable<Seed> CreateContextRoleProperties(Usage usage, RoleSeed role)
        {
            yield return new RolePropertySeed(
                usage.Type.Identifier.TrimStart('I'),
                NS.CreateGlobalPath(usage.Namespace, role),
                Concerns: new[]
                {
                    DefaultRoleConcerns.AssignContextConcern(usage.Type)
                }
            );
        }

        static IEnumerable<Seed> CreateRoleToContextMemberSeeds(Usage usage, ClassSeed theContext)
        {
            yield return new PropertySeed(
                "Context",
                NS.CreateGlobalPath(usage.Namespace, theContext.Identifier),
                ImmutableList<ConcernSeed>.Empty,
                ImmutableList<ValidationSeed>.Empty,
                false,
                IncludeInCtor: false,
                SpecificInterface: new InterfaceSeed(usage.Type.Identifier)
            );
        }
    }

    private IReadOnlyList<NamespaceSeed> AddImplicitConversions(IReadOnlyList<NamespaceSeed> namespaceSeeds)
    {
        var addImplicitConversions = new AddImplicitConversion(_properties);

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
            })
            .ToList();
    }

    private IReadOnlyList<NamespaceSeed> AddRoles(IReadOnlyList<NamespaceSeed> namespaceSeeds)
    {
        var addInterfaceImplementation = new AddInterfaceImplementation(_properties);
        var roleSpaces = namespaceSeeds.Where(x => x.Clazz is RoleSeed).ToArray();
        return namespaceSeeds
            .Select(ns => roleSpaces.Aggregate(ns, addInterfaceImplementation.ViaRole))
            .ToList();
    }
}

public interface A
{
    public int aa { get; set; }
}

public interface B
{
    public int aa { get; set; }
}

public class C : A, B
{
    int A.aa { get; set; }
    int B.aa { get; set; }
}