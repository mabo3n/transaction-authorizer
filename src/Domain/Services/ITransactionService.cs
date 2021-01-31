using System.Collections.Generic;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;

namespace Authorizer.Domain.Services
{
    public interface ITransactionService
    {
        IEnumerable<Violation> Authorize(Transaction transaction);
        Account Apply(Transaction transaction);
    }
}
