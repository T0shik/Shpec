using Microsoft.CodeAnalysis;

namespace Shpec.Generators.Utils;

public class ShpecAggregationException : Exception
{
    public ShpecAggregationException(string message, SyntaxNode node)
        : base(($"{message}. Syntax: {node.ToString().ReplaceLineEndings("")}"))
    {
    }
}

public class ShpecGenerationException : Exception
{
    public ShpecGenerationException(string message) : base(message)
    {
    }
}