using System;
using System.Collections.Generic;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;

namespace Authorizer.Infrastructure
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDataSource dataSource;

        public TransactionRepository(IDataSource dataSource)
            => this.dataSource = dataSource;

        public IEnumerable<Transaction> QueryByTimeWindow(
            DateTime from, DateTime to
        )
            => dataSource.Transactions
                .Where(t => t.Time > from && t.Time < to);

        public void Save(Transaction transaction)
            => dataSource.Transactions = dataSource.Transactions
                .Append(transaction);
    }
}
