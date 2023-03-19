using Microsoft.CodeAnalysis;

namespace Shpec.Generator.Utils;

public class ShpecAggregationException : Exception
{
    public ShpecAggregationException(string message, SyntaxNode node)
        : base(($"{message}. Syntax: {node.ToString().Replace(Environment.NewLine, "")}"))
    {
    }
}

public class ShpecTranslationException : Exception
{
    public ShpecTranslationException(string message)
        : base(message)
    {
    }
}

public class ShpecGenerationException : Exception
{
    public ShpecGenerationException(string message) : base(message)
    {
    }
}