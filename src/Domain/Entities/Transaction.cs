using System;

namespace Authorizer.Domain.Entities
{
    public class Transaction : IEquatable<Transaction>
    {
        public Guid Id { get; }

        public string Merchant { get; }
        public int Amount { get; }
        public DateTime Time { get; }

        public Transaction(string merchant, int amount, DateTime time)
        {
            Id = Guid.NewGuid();

            Merchant = merchant;
            Amount = amount;
            Time = time;
        }

        public bool IsSimilarTo(Transaction other)
            => other != null
            && this.Merchant == other.Merchant
            && this.Amount == other.Amount;

        public bool Equals(Transaction other)
            => other != null
            && this.Id == other.Id;
    }
}
