using System.Collections.Immutable;
using Shpec.Generator.Utils;

namespace Shpec.Generator.Functions;

internal class AddInterfaceImplementation
{
    private readonly List<PropertyDefinition> _propertyDefinitions;

    internal AddInterfaceImplementation(List<PropertyDefinition> propertyDefinitions)
    {
        _propertyDefinitions = propertyDefinitions;
    }

    public NamespaceSeed ViaRole(NamespaceSeed targetSpace, NamespaceSeed fromSpace)
    {
        if (targetSpace.Clazz is not ClassSeed target)
        {
            return targetSpace;
        }

        // records & structs playing roles not supported.
        if (target.Record || target.Struct)
        {
            return targetSpace;
        }

        if (fromSpace.Clazz is not RoleSeed from)
        {
            return targetSpace;
        }

        var roleProps = from.Members.OfType<PropertySeed>().ToList();
        if (!HasAllProperties(
                target.Members.OfType<PropertySeed>().ToList(),
                roleProps
            ))
        {
            return targetSpace;
        }

        var forceImplementation = roleProps
            .Where(p => p.SpecificInterface != null)
            .Select(p => p with
            {
                Identifier = NS.CreateGlobalPath(fromSpace, appendix:p.Identifier),
                DeclarationSpecificToInterface = true,
            })
            .ToList();

        return targetSpace with
        {
            Clazz = target with
            {
                Interfaces = target.Interfaces
                    .Add(new InterfaceSeed(NS.CreateGlobalPath(fromSpace))),
                Members = forceImplementation.Count == 0
                    ? target.Members
                    : target.Members.Concat(forceImplementation).ToImmutableList()
            },
        };
    }

    private static bool HasAllProperties(
        IReadOnlyList<PropertySeed> targetProperties,
        IReadOnlyList<PropertySeed> interfaceProperties
    )
    {
        return interfaceProperties
            .All(x =>
                x.SpecificInterface != null
                || targetProperties.Any(t => x.Identifier == t.Identifier)
            );
    }
}