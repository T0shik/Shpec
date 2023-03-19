namespace Shpec.Generator.Utils;

internal static class NS
{
    internal static string CreateGlobalPath(
        NamespaceSeed from,
        TypeSeed? seed = null,
        string appendix = ""
    ) =>
        CreateGlobalPath(from.Identifier, seed ?? from.Clazz, appendix);

    internal static string CreateGlobalPath(
        string ns,
        TypeSeed seed,
        string appendix = ""
    ) =>
        CreateGlobalPath(ns, RecurPath(seed), appendix);

    internal static string CreateGlobalPath(
        string ns,
        string typePath,
        string appendix = ""
    ) =>
        string.IsNullOrEmpty(appendix)
            ? $"global::{ns}.{typePath}"
            : $"global::{ns}.{typePath}.{appendix}";

    internal static string RecurPath(TypeSeed seed) => seed.Parent == null
        ? seed.Identifier
        : RecurPath(seed.Parent) + "." + seed.Identifier;
}