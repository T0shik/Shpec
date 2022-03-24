using Microsoft.CodeAnalysis;

namespace Shpec.Generators;

public static class SyntaxNodeExtensions
{
    public static T? GetParent<T>(this SyntaxNode @this, bool throwError = false)
        where T : SyntaxNode
    {
        var parent = @this.Parent;
        while (true)
        {
            if (parent == null)
            {
                if (throwError)
                {
                    throw new Exception($"Failed to find parent {typeof(T).Name}, for {@this.GetType().Name}.\n{@this.GetText()}");
                }
                return null;
            }

            if (parent is T t)
            {
                return t;
            }

            parent = parent.Parent;
        }
    }
}