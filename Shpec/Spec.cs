// ReSharper disable InconsistentNaming

using System.Numerics;
using Microsoft.VisualBasic.CompilerServices;

namespace Shpec;

public partial class Member
{
    public static readonly Member Default = new();
    public static implicit operator bool(Member p) => default;
    public static implicit operator short(Member p) => 0;
    public static implicit operator ushort(Member p) => 0;
    public static implicit operator int(Member p) => 0;
    public static implicit operator uint(Member p) => 0;
    public static implicit operator long(Member p) => 0;
    public static implicit operator ulong(Member p) => 0;
    public static implicit operator float(Member p) => 0;
    public static implicit operator double(Member p) => 0;
    public static implicit operator decimal(Member p) => 0;
    public static implicit operator BigInteger(Member p) => 0;
    public static implicit operator char(Member p) => char.MinValue;
    public static implicit operator string(Member p) => string.Empty;
    public static implicit operator TimeOnly(Member p) => TimeOnly.MinValue;
    public static implicit operator DateOnly(Member p) => DateOnly.MinValue;
    public static implicit operator DateTime(Member p) => DateTime.MinValue;
    public static implicit operator DateTimeOffset(Member p) => DateTimeOffset.MinValue;
    public static implicit operator short[](Member p) => Array.Empty<short>();
    public static implicit operator ushort[](Member p) => Array.Empty<ushort>();
    public static implicit operator int[](Member p) => Array.Empty<int>();
    public static implicit operator uint[](Member p) => Array.Empty<uint>();
    public static implicit operator long[](Member p) => Array.Empty<long>();
    public static implicit operator ulong[](Member p) => Array.Empty<ulong>();
    public static implicit operator float[](Member p) => Array.Empty<float>();
    public static implicit operator double[](Member p) => Array.Empty<double>();
    public static implicit operator decimal[](Member p) => Array.Empty<decimal>();
    public static implicit operator BigInteger[](Member p) => Array.Empty<BigInteger>();
    public static implicit operator char[](Member p) => Array.Empty<char>();
    public static implicit operator string[](Member p) => Array.Empty<string>();
    public static implicit operator TimeOnly[](Member p) => Array.Empty<TimeOnly>();
    public static implicit operator DateOnly[](Member p) => Array.Empty<DateOnly>();
    public static implicit operator DateTime[](Member p) => Array.Empty<DateTime>();
    public static implicit operator DateTimeOffset[](Member p) => Array.Empty<DateTimeOffset>();
}

public static class Declare
{
    public static Member _property(bool immutable = false) => Member.Default;
    public static Property<T> _property<T>(bool immutable = false) => new();
    public static Member _computed(object o) => new();
    public static Computed<T> _computed<T>(object o) => new();
}

public class Property<T> : Member
{
    public static T must(Func<T, bool> predicate) => default;
    public static implicit operator T(Property<T> T) => default;
}

public class Computed<T> : Property<T> { }

public class Properties
{
    public Properties(params object[] members) { }
}

public static class ValidationExtensions
{
    public static Member must<T>(this T _, Func<T, bool> predicate) => Member.Default;
    public static Member match<T>(string regex) => Member.Default;
}