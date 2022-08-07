using System.Runtime.CompilerServices;
using Shpec;
using Shpec.Declare;

namespace Playground.UseCases
{
    using Playground.UseCases.DCI.MoneyTransfer;

    // Data Context Interaction (https://en.wikipedia.org/wiki/Data,_context_and_interaction)
    // Referenced C# Example (https://github.com/programmersommer/DCISample)
    // Money Transfer Example
    public class dci : IUseCase
    {
        public void Execute()
        {
            var a1 = new Account(100);
            var a2 = new Account(10);
            var ctx = new MoneyTransferContext();
            ctx.Transfer(a1, a2, 50);
            if (a1.Amount != 50 && a2.Amount != 60)
            {
                throw new Exception("failed transfer");
            }
        }
    }
}

namespace Playground.UseCases.DCI.MoneyTransfer
{
    using static TestMembers;

    public class TestMembers
    {
        public static int Amount = Member<int>.Property();

        [MethodDefinition]
        public static void AddFunds(int amount)
        {
            Amount += amount;
        }

        [MethodDefinition]
        public static void SubtractFunds(int amount)
        {
            Amount -= amount;
        }
    }

    public partial class Account
    {
        Members _m => new(Amount);
    }

    public partial class MoneyTransferContext
    {
        public void Transfer(Source source, Destination destination, int amount)
        {
            if (source.Amount < amount)
            {
                throw new InvalidOperationException("insufficient funds");
            }

            source.SubtractFunds(amount);
            destination.AddFunds(amount);
        }

        public partial class Source
        {
            Members _m => new(Amount, SubtractFunds);
        }

        public partial class Destination
        {
            Members _m => new(Amount, AddFunds);
        }
    }
}