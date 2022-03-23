using System;
using Microsoft.CodeAnalysis;

namespace Shpec.Generators;

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
}