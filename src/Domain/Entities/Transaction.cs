using System;

namespace Authorizer.Domain.Entities
{
    public class Transaction
    {
        public string Merchant { get; }
        public int Amount { get; }
        public DateTime Time { get; }

        public Transaction(string merchant, int amount, DateTime time)
        {
            Merchant = merchant;
            Amount = amount;
            Time = time;
        }
    }
}
