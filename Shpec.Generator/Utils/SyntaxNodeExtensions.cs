using Microsoft.CodeAnalysis;

namespace Shpec.Generator;

public static class SyntaxNodeExtensions
{
    public static T GetParent<T>(this SyntaxNode @this)
        where T : SyntaxNode
    {
        var parent = @this.Parent;
        while (true)
        {
            if (parent == null)
            {
                throw new Exception($"Failed to find parent {typeof(T).Name}, for {@this.GetType().Name}.\n{@this.GetText()}");
            }

            if (parent is T t)
            {
                return t;
            }

            parent = parent.Parent;
        }
    }

    public static T? TryGetParent<T>(this SyntaxNode @this)
        where T : SyntaxNode
    {
        var parent = @this.Parent;
        while (true)
        {
            if (parent == null)
            {
                return null;
            }

            if (parent is T t)
            {
                return t;
            }

            parent = parent.Parent;
        }
    }
    
    public static T? FindChild<T>(this SyntaxNode @this)
        where T : SyntaxNode
    {
        foreach (var child in @this.ChildNodes())
        {
            if (child is T x)
            {
                return x;
            }

            var nested = child.FindChild<T>();
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }
}