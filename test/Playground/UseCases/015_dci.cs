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
            var (sourceId, destinationId, amount) = request;
            SourceAccount = _bank.Get(sourceId);
            SourceAccount.Context = this;
            DestinationAccount = _bank.Get(destinationId);
            DestinationAccount.Context = this;

            SourceAccount.Transfer(amount);
            
            _bank.Set(SourceAccount);
            _bank.Set(DestinationAccount);
        }
        
        private Source SourceAccount { get; set; }
        private Destination DestinationAccount { get; set; }
        private static MoneyTransferContext Context => Member<MoneyTransferContext>.Property();

        public partial class Source
        {
            Members _m => new(Context, AccountId, Amount, SubtractFunds);

            public void Transfer(int amount)
            {
                if (Amount < amount)
                {
                    throw new InvalidOperationException("insufficient funds");
                }

                SubtractFunds(amount);
                Context.DestinationAccount.Transfer(amount);
            }
        }

        public partial class Destination
        {
            Members _m => new(Context, Amount, AddFunds);
            
            public void Transfer(int amount)
            {
                AddFunds(amount);
            }
        }
    }
}