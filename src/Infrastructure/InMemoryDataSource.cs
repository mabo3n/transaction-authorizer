using System.Linq;
using Authorizer.Domain.Entities;

namespace Authorizer.Infrastructure
{
    public interface IDataSource
    {
        Account Account { get; set; }
        IQueryable<Transaction> Transactions { get; set; }
    }

    public class InMemoryDataSource : IDataSource
    {
        public Account Account { get; set; }
        public IQueryable<Transaction> Transactions { get; set; }
            = Enumerable.Empty<Transaction>().AsQueryable();
    }
}
