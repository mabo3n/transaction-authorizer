using System.Threading.Tasks;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;

namespace Authorizer.Infrastructure
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataSource dataSource;

        public AccountRepository(IDataSource dataSource)
            => this.dataSource = dataSource;

        public Task<Account> Get()
            => Task.FromResult(dataSource.Account);

        public Task Save(Account account)
            => Task.FromResult(dataSource.Account = account);
    }
}
