using System;
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

        private List<Transaction> GetPrecedingTransactions(
            DateTime time,
            int intervalInMinutes
        )
            => transactionRepository
                .QueryByTimeWindow(
                    from: time.AddMinutes(-intervalInMinutes),
                    to: time
                ).ToList();

        public IEnumerable<Violation> Authorize(
            Transaction transaction, Account account
        )
        {
            if (account.ActiveCard == false)
                yield return Violation.CardNotActive;

            if (transaction.Amount > account.AvailableLimit)
                yield return Violation.InsufficientLimit;

            var precedingTransactions = GetPrecedingTransactions(
                transaction.Time,
                intervalInMinutes: 2
            );

            if (precedingTransactions.Any(t => t.IsSimilarTo(transaction)))
                yield return Violation.DoubledTransaction;

            var highFrequencyTransactionCount = 3;

            if (precedingTransactions.Count() >= highFrequencyTransactionCount)
                yield return Violation.HighFrequencySmallInterval;

            yield break;
        }
    }
}
