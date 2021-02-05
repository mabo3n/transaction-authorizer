using System.Threading.Tasks;
using Authorizer.Domain.Entities;

namespace Authorizer.Domain.Repositories
{
    public interface IAccountRepository
    {
        public Task<Account> Get();
        public Task Save(Account Account);
    }
}
