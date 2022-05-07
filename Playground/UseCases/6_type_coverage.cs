using System.Numerics;
using Shpec;
using static Shpec.Declare;

namespace Playground.UseCases;

public partial class type_coverage : IUseCase
{
    Properties _p =>
        new(
            Short,
            Int,
            Long,
            UShort,
            UInt,
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
            D4teT1meOffset
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
    }

    public void Execute()
    {
        var e = new type_coverage()
        {
            Short = 0,
            Int = 0,
            Long = 0,
            UShort = 0,
            UInt = 0,
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
        };
    }
}