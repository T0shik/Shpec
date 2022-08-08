namespace Shpec.Generator.Utils;

public static class StringExtensions
{
    public static string LastAccessor(this string v) => v.Split('.').Last();
}