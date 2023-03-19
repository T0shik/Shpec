using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace Shpec.Generator.Utils;


public class ShpecAggregationException : Exception
{
    public ShpecAggregationException(
        string message, 
        SyntaxNode node, 
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "",
        [CallerLineNumber] int lineNumber = 0
        )
        : base($"[{fileName}#{lineNumber}::{memberName}] {message}. Syntax: {node.ToString().Replace(Environment.NewLine, "")}")
    {
    }
}

public class ShpecTranslationException : Exception
{
    public ShpecTranslationException(
        string message, 
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "",
        [CallerLineNumber] int lineNumber = 0
        )
        : base($"[{fileName}#{lineNumber}::{memberName}] {message}")
    {
    }
}

public class ShpecGenerationException : Exception
{
    public ShpecGenerationException(
        string message, 
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "",
        [CallerLineNumber] int lineNumber = 0
        ) 
        : base($"[{fileName}#{lineNumber}::{memberName}] {message}")
    {
    }
}