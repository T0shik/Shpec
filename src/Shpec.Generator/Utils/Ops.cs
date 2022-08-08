namespace Shpec.Generator.Utils;

internal static class Ops
{
    public static void Try(string description, Action operation)
    {
        try
        {
            operation();
        }
        catch (Exception ex)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "error.log");
            File.WriteAllText(path, description + Environment.NewLine + Environment.NewLine + ex.ToString());
            throw;
        }
    }
}
