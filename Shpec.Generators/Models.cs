using Microsoft.CodeAnalysis.CSharp;

namespace Shpec.Generators;

public record PropertyInfo(string Type, string Identifier, SyntaxKind ValueType);

public record SchemaDeclaration(string NameSpace, string ParentClass, string Clazz, IEnumerable<string> Properties);