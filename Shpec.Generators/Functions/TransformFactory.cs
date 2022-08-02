namespace Shpec.Generators.Functions;

internal class TransformFactory
{
    private readonly List<PropertyDefinition> _propertyDefinitions;
    private readonly List<ComputedPropertyDefinition> _computedPropertyDefinitions;

    public TransformFactory(
        List<PropertyDefinition> propertyDefinitions, 
        List<ComputedPropertyDefinition> computedPropertyDefinitions
        )
    {
        _propertyDefinitions = propertyDefinitions;
        _computedPropertyDefinitions = computedPropertyDefinitions;
    }

    internal StatementTransformer StatementTransformer() => new(this);

    internal PropertyExpressionTransformer PropertyExpressionTransformer() => new(this, _propertyDefinitions, _computedPropertyDefinitions);
}
