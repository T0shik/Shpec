using System.Collections;
using System.Runtime.CompilerServices;
using Shpec;
using Shpec.Declare;

namespace Playground.UseCases
{
    using DCI.MoneyTransfer;

    // Data Context Interaction (https://en.wikipedia.org/wiki/Data,_context_and_interaction)
    // Referenced C# Example (https://github.com/programmersommer/DCISample)
    // Money Transfer Example
    public class dci : IUseCase
    {
        public void Execute()
        {
            var bank = new Bank();
            bank.Set(new Account(1, 100));
            bank.Set(new Account(2, 10));
            var ctx = new MoneyTransferContext(bank);
            ctx.Transfer(new(1, 2, 50));

            if (bank.Get(1).Amount != 50 && bank.Get(2).Amount != 60)
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
        public static int AccountId = Member<int>.Property();
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
        Members _m => new(AccountId, Amount);
    }

    public class Bank
    {
        private readonly Dictionary<int, Account> _accounts = new() { };
        public Account Get(int id) => _accounts[id];
        public void Set(Account account) => _accounts[account.AccountId] = account;
    }

    // todo: concerns to ponder
    // - we create a new object rather than cast, this could be expensive, maybe a decorator, or some interface madness?
    // - because we can't reach context from role it goes in the constructor, there is a fusion going on here.
    //   assigning the Context isn't hard but it didn't bring joy.
    //   almost want to have the class span multiple objects, sounds like an abomination. 
    //   or the role should be a framework unit that we jam the object into.
    // - This whole thing is tied to the Account class, making the role void.
    public partial class MoneyTransferContext
    {
        private readonly Bank _bank;

        public MoneyTransferContext(Bank bank)
        {
            _bank = bank;
        }

        public record TransferRequest(int SourceId, int DestinationId, int Amount);

        public void Transfer(TransferRequest request)
        {
            // set the scene
            var (sourceId, destinationId, amount) = request;
            SourceAccount = _bank.Get(sourceId);
            DestinationAccount = _bank.Get(destinationId);

            // start play
            SourceAccount.Transfer(amount);
            
            // clean up
            _bank.Set(SourceAccount);
            _bank.Set(DestinationAccount);
        }

        public partial class SourceAccountRole
        {
            Role _m => new(AccountId, Amount, SubtractFunds);

            public void Transfer(int amount)
            {
                // interact
                if (Amount < amount)
                {
                    throw new InvalidOperationException("insufficient funds");
                }

                SubtractFunds(amount);
                Context.DestinationAccount.Transfer(amount);
            }
        }

        public partial class DestinationAccountRole
        {
            Role _m => new(Amount, AddFunds);
            
            public void Transfer(int amount)
            {
                // interact
                AddFunds(amount);
            }
        }
    }
}