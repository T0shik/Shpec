﻿using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Shpec;
using Shpec.Declare;

namespace Playground.UseCases
{
    using DCI.MoneyTransfer;

    // Data Context Interaction (https://en.wikipedia.org/wiki/Data,_context_and_interaction)
    // Referenced C# Example (https://github.com/programmersommer/DCISample)
    // Money Transfer Example
    public class dci_money_transfer : IUseCase
    {
        public void Execute()
        {
            var bank = new Bank();
            bank.Set(new Account(1, 100));
            bank.Set(new Account(2, 10));
            var ctx = new MoneyTransferContext(bank, 1, 2);
            ctx.Transfer(50);

            Debug.Assert(bank.Get(1).Amount == 50, "subtracted money from source account");
            Debug.Assert(bank.Get(2).Amount == 60, "added money from source account");
        }
    }
}

namespace Playground.UseCases.DCI.MoneyTransfer
{
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
        public MoneyTransferContext(Bank bank, int sourceId, int destinationId)
        {
            SourceAccount = bank.Get(sourceId);
            DestinationAccount = bank.Get(destinationId);
        }

        public void Transfer(int amount)
        {
            SourceAccount.Transfer(amount);
        }

        public partial interface ISourceAccount
        {
            Members _m => new(AccountId, Amount, SubtractFunds);

            public void Transfer(int amount)
            {
                if (Amount < amount)
                {
                    throw new InvalidOperationException("insufficient funds");
                }

                SubtractFunds(amount);
                // interact
                Context.DestinationAccount.Transfer(amount);
            }
        }

        public partial interface IDestinationAccount
        {
            Members _m => new(Amount, AddFunds);

            public void Transfer(int amount)
            {
                // interact
                AddFunds(amount);
            }
        }
    }
}