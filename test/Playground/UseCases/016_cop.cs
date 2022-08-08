using System.Collections;
using System.Runtime.CompilerServices;
using Shpec;
using Shpec.Declare;

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
    }

    public partial class Division
    {
        Members _m => new Members(
            Number1,
            Number2,
            Divide
        );
    }
}