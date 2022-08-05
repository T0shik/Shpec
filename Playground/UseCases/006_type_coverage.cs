using System.Numerics;
using Shpec;
using Shpec.Declare;

namespace Playground.UseCases;

public partial class type_coverage : IUseCase
{
    Members _p =>
        new(
            Boole4n,
            Short,
            UShort,
            Int,
            UInt,
            Long,
            ULong,
            Flo,
            Dub,
            Dec,
            Gu1d,
            BigInt,
            BigIntQual,
            Chr,
            Str,
            T1me,
            D4te,
            D4teT1me,
            D4teT1meOffset,
            ShortArr,
            UShortArr,
            IntArr,
            UIntArr,
            LongArr,
            ULongArr,
            FloArr,
            DubArr,
            DecArr,
            Gu1dArr,
            BigIntArr,
            BigIntQualArr,
            ChrArr,
            StrArr,
            T1meArr,
            D4teArr,
            D4teT1meArr,
            D4teT1meOffsetArr
        );

    public class Props
    {
        static bool Boole4n => Member<bool>.Property();
        static short Short => Member<short>.Property();
        static ushort UShort => Member<ushort>.Property();
        static int Int => Member<int>.Property();
        static uint UInt => Member<uint>.Property();
        static long Long => Member<long>.Property();
        static ulong ULong => Member<ulong>.Property();
        static float Flo => Member<float>.Property();
        static double Dub => Member<double>.Property();
        static decimal Dec => Member<decimal>.Property();
        static Guid Gu1d => Member<Guid>.Property();
        static BigInteger BigInt => Member<BigInteger>.Property();
        private static BigInteger BigIntQual => Member<BigInteger>.Property();
        static char Chr => Member<char>.Property();
        static string Str => Member<string>.Property();
        static TimeOnly T1me => Member<TimeOnly>.Property();
        static DateOnly D4te => Member<DateOnly>.Property();
        static DateTime D4teT1me => Member<DateTime>.Property();
        static DateTimeOffset D4teT1meOffset => Member<DateTimeOffset>.Property();
        static short[] ShortArr => Member<short[]>.Property();
        static ushort[] UShortArr => Member<ushort[]>.Property();
        static int[] IntArr => Member<int[]>.Property();
        static uint[] UIntArr => Member<uint[]>.Property();
        static long[] LongArr => Member<long[]>.Property();
        static ulong[] ULongArr => Member<ulong[]>.Property();
        static float[] FloArr => Member<float[]>.Property();
        static double[] DubArr => Member<double[]>.Property();
        static decimal[] DecArr => Member<decimal[]>.Property();
        static BigInteger[] BigIntArr => Member<BigInteger[]>.Property();
        static Guid[] Gu1dArr => Member<Guid[]>.Property();
        static BigInteger[] BigIntQualArr => Member<BigInteger[]>.Property();
        static char[] ChrArr => Member<char[]>.Property();
        static string[] StrArr => Member<string[]>.Property();
        static TimeOnly[] T1meArr => Member<TimeOnly[]>.Property();
        static DateOnly[] D4teArr => Member<DateOnly[]>.Property();
        static DateTime[] D4teT1meArr => Member<DateTime[]>.Property();
        static DateTimeOffset[] D4teT1meOffsetArr => Member<DateTimeOffset[]>.Property();
    }

    public void Execute()
    {
        var e = new type_coverage()
        {
            Boole4n = true,
            Short = 0,
            UShort = 0,
            Int = 0,
            UInt = 0,
            Long = 0,
            ULong = 0,
            Flo = 0,
            Dub = 0,
            Dec = 0,
            Gu1d = Guid.Empty,
            BigInt = 0,
            BigIntQual = 0,
            Chr = 'a',
            Str = "",
            T1me = TimeOnly.MaxValue,
            D4te = DateOnly.MaxValue,
            D4teT1me = DateTime.Now,
            D4teT1meOffset = DateTimeOffset.Now,
            ShortArr = Array.Empty<short>(),
            UShortArr = Array.Empty<ushort>(),
            IntArr = Array.Empty<int>(),
            UIntArr = Array.Empty<uint>(),
            LongArr = Array.Empty<long>(),
            ULongArr = Array.Empty<ulong>(),
            FloArr = Array.Empty<float>(),
            DubArr = Array.Empty<double>(),
            DecArr = Array.Empty<decimal>(),
            Gu1dArr = Array.Empty<Guid>(),
            BigIntArr = Array.Empty<BigInteger>(),
            BigIntQualArr = Array.Empty<BigInteger>(),
            ChrArr = Array.Empty<char>(),
            StrArr = Array.Empty<string>(),
            T1meArr = Array.Empty<TimeOnly>(),
            D4teArr = Array.Empty<DateOnly>(),
            D4teT1meArr = Array.Empty<DateTime>(),
            D4teT1meOffsetArr = Array.Empty<DateTimeOffset>(),
        };
    }
}