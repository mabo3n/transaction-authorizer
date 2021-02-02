using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;

namespace Authorizer.Infrastructure
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataSource dataSource;

        public AccountRepository(IDataSource dataSource)
            => this.dataSource = dataSource;

        public void Save(Account account)
            => dataSource.Account = account;
    }
}
