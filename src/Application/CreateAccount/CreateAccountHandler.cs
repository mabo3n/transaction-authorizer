using System.Threading.Tasks;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using Authorizer.Domain.Repositories;

namespace Authorizer.Application
{
    public class CreateAccountHandler : IOperationHandler<CreateAccount>
    {
        private readonly IAccountRepository accountRepository;

        public CreateAccountHandler(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<OperationResult> Handle(CreateAccount payload)
        {
            var existingAccount = await accountRepository.Get();

            if (existingAccount != null)
                return new OperationResult(
                    existingAccount,
                    violations: new Violation[] {
                        Violation.AccountAlreadyInitialized
                    }
                );

            var newAccount = new Account(
                availableLimit: payload.AvailableLimit,
                activeCard: payload.ActiveCard
            );

            await accountRepository.Save(newAccount);

            return new OperationResult(
                account: newAccount,
                violations: new Violation[] { }
            );
        }
    }
}
