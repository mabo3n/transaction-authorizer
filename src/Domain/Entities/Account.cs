using System;

namespace Authorizer.Domain.Entities
{
    public class Account
    {
        public bool ActiveCard { get; }
        public int AvailableLimit { get; }

        public Account(int availableLimit, bool activeCard)
        {
            if (availableLimit < 0) throw new InvalidOperationException(
                $"Account can't have a negative {nameof(availableLimit)}"
            );

            AvailableLimit = availableLimit;
            ActiveCard = activeCard;
        }

        public Account Apply(Transaction transaction)
            => ActiveCard
                ? new Account(
                    this.AvailableLimit - transaction.Amount,
                    this.ActiveCard
                )
                : throw new InvalidOperationException(
                    "Account card is not active"
                );
    }
}
