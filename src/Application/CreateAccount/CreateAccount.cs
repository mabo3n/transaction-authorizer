using Authorizer.Domain.Entities;

namespace Authorizer.Application
{
    public class CreateAccount : Operation
    {
        public Account Account { get; }

        public CreateAccount(Account account)
        {
            Account = account;
        }
    }
}
