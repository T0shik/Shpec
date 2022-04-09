namespace Shpec.Generators.Functions;

internal class TransformFactory
{
    private List<PropertyDefinition> _propertyDefinitions;
    
    public TransformFactory(List<PropertyDefinition> propertyDefinitions)
    {
        _propertyDefinitions = propertyDefinitions;
    }

    internal TransformStatements TransformStatements()
    {
        return new TransformStatements(this);
    }

    internal TransformExpression TransformComputedPropertyExpression()
    {
        return new TransformExpression(this, _propertyDefinitions);
    }
}
