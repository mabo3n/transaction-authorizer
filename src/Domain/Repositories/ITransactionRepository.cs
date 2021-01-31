using System;
using System.Collections.Generic;
using Authorizer.Domain.Entities;

namespace Authorizer.Domain.Repositories
{
    public interface ITransactionRepository
    {
        public IEnumerable<Transaction> QueryByTimeWindow(
            DateTime from, DateTime to
        );
    }
}
