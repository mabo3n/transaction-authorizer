using System;
using System.Linq;

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

        private Account Apply(Transaction transaction)
            => ActiveCard
                ? new Account(
                    AvailableLimit - transaction.Amount,
                    ActiveCard
                )
                : throw new InvalidOperationException(
                    "Account card is not active"
                );

        public Account Apply(params Transaction[] transactions)
            => transactions.Aggregate(
                this,
                (account, transaction) => account.Apply(transaction)
            );
    }
}
