using Authorizer.Domain.Entities;

namespace Authorizer.Domain.Repositories
{
    public interface IAccountRepository
    {
        public Account Get();
        public void Save(Account Account);
    }
}
