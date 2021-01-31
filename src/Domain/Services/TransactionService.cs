using System.Collections.Generic;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using Authorizer.Domain.Repositories;

namespace Authorizer.Domain.Services
{
    public class TransactionService : ITransactionService
    {
        private ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public IEnumerable<Violation> Authorize(Transaction transaction)
        {
            yield break;
        }

        public Account Apply(Transaction transaction)
        {
            return null;
        }
    }
}
