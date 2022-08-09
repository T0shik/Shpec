using Shpec;
using Shpec.Declare;
using Shpec.Validation;

namespace Playground.UseCases
{
    using COP.Numbers;

    // Composite Oriented Programming https://polygene.apache.org/java/latest/what-is-cop.html
    // kinda like Aspect Oriented
    public class cop : IUseCase
    {
        public void Execute()
        {
            var division = new Division(10, 2);
            var result = division.Divide();
            if (result != 5)
            {
                throw new("bad math");
            }

            var division2 = new Division();
            try
            {
                division2.Number2 = 0;
                throw new("failed to set");
            }
            catch (ArgumentException)
            {
            }
        }
    }
}

namespace Playground.UseCases.COP.Numbers
{
    using static TestMembers;

    public class TestMembers
    {
        public static int Number1 => Member<int>.Property();
        public static int Number2 => Member<int>.Property();

        [MethodDefinition]
        public static int Divide()
        {
            return Number1 / Number2;
        }

        [MethodDefinition]
        public static void ThrowIfZero(int v)
        {
            if (v == 0)
            {
                throw new ArgumentException();
            }
        }
    }

    public partial class Division
    {
        Members _m => new Members(
            Number1,
            Concern.For(Number2).BeforeSet(ThrowIfZero),
            Divide
        );
    }
}