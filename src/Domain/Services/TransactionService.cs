using System.Collections.Generic;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using Authorizer.Domain.Repositories;

namespace Authorizer.Domain.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public IEnumerable<Violation> Authorize(
            Transaction transaction, Account account
        )
        {
            if (account.ActiveCard == false)
                yield return Violation.CardNotActive;

            if (transaction.Amount > account.AvailableLimit)
                yield return Violation.InsufficientLimit;

            var twoMinutesBefore = transaction.Time.AddMinutes(-2);
            var precedingTransactions = transactionRepository
                .QueryByTimeWindow(
                    from: twoMinutesBefore, to: transaction.Time
                ).ToList();

            if (precedingTransactions.Count() >= 3)
                yield return Violation.HighFrequencySmallInterval;

            if (precedingTransactions.Any(t => t.IsSimilarTo(transaction)))
                yield return Violation.DoubledTransaction;

            yield break;
        }
    }
}
