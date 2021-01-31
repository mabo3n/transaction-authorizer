using System;
using System.Collections.Generic;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;

namespace Authorizer.Infrastructure
{
    public class TransactionRepository : ITransactionRepository
    {
        private IDataSource DataSource { get; }

        public TransactionRepository(IDataSource dataSource)
            => DataSource = dataSource;

        public IEnumerable<Transaction> QueryByTimeWindow(
            DateTime from,
            DateTime to
        )
            => DataSource.Transactions
                .Where(t => t.Time > from && t.Time < to);

        public void Save(Transaction transaction)
            => DataSource.Transactions = DataSource.Transactions
                .Append(transaction);
    }
}
