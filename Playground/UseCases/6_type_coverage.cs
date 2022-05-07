using System.Numerics;
using Shpec;
using static Shpec.Declare;

namespace Playground.UseCases;

public partial class type_coverage : IUseCase
{
    Properties _p =>
        new(
            Short,
            UShort,
            Int,
            UInt,
            Long,
            ULong,
            Flo,
            Dub,
            Dec,
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
        static short Short => _property();
        static ushort UShort => _property();
        static int Int => _property();
        static uint UInt => _property();
        static long Long => _property();
        static ulong ULong => _property();
        static float Flo => _property();
        static double Dub => _property();
        static decimal Dec => _property();
        static BigInteger BigInt => _property();
        static System.Numerics.BigInteger BigIntQual => _property();
        static char Chr => _property();
        static string Str => _property();
        static TimeOnly T1me => _property();
        static DateOnly D4te => _property();
        static DateTime D4teT1me => _property();
        static DateTimeOffset D4teT1meOffset => _property();
        static short[] ShortArr => _property();
        static ushort[] UShortArr => _property();
        static int[] IntArr => _property();
        static uint[] UIntArr => _property();
        static long[] LongArr => _property();
        static ulong[] ULongArr => _property();
        static float[] FloArr => _property();
        static double[] DubArr => _property();
        static decimal[] DecArr => _property();
        static BigInteger[] BigIntArr => _property();
        static System.Numerics.BigInteger[] BigIntQualArr => _property();
        static char[] ChrArr => _property();
        static string[] StrArr => _property();
        static TimeOnly[] T1meArr => _property();
        static DateOnly[] D4teArr => _property();
        static DateTime[] D4teT1meArr => _property();
        static DateTimeOffset[] D4teT1meOffsetArr => _property();
    }

    public void Execute()
    {
        var e = new type_coverage()
        {
            Short = 0,
            UShort = 0,
            Int = 0,
            UInt = 0,
            Long = 0,
            ULong = 0,
            Flo = 0,
            Dub = 0,
            Dec = 0,
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