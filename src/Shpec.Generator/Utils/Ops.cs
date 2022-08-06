using Microsoft.CodeAnalysis.CSharp;

namespace Shpec.Generator.Utils;

internal static class Ops
{
    public static void Try(Action operation, string description)
    {
        try
        {
            operation();
        }
        catch (Exception ex)
        {
            throw new Exception(description + " >>> Original: " + ex.Message);
        }
    }

    public static T Try<T>(Func<T> operation, string description)
    {
        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            throw new Exception(description + " >>> Original: " + ex.Message);
        }
    }

    public static SyntaxKind Assert(this SyntaxKind @this, params SyntaxKind[] matches) 
    {
        if (!matches.Contains(@this))
        {
            throw new Exception("unexpected SyntaxKind");
        }

        return @this;
    }
}
