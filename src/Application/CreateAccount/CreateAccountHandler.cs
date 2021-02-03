using System.Threading;
using System.Threading.Tasks;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using Authorizer.Domain.Repositories;
using MediatR;

namespace Authorizer.Application
{
    public class CreateAccountHandler
        : IRequestHandler<CreateAccount, OperationResult>
    {
        private readonly IAccountRepository accountRepository;

        public CreateAccountHandler(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public Task<OperationResult> Handle(
            CreateAccount payload,
            CancellationToken cancellationToken
        )
        {
            var existingAccount = accountRepository.Get();

            if (existingAccount != null)
                return Task.FromResult(
                    new OperationResult(
                        account: existingAccount,
                        violations: new Violation[] {
                            Violation.AccountAlreadyInitialized
                        }
                    )
                );

            var newAccount = new Account(
                availableLimit: payload.AvailableLimit,
                activeCard: payload.ActiveCard
            );
            accountRepository.Save(newAccount);

            return Task.FromResult(
                new OperationResult(
                    account: newAccount,
                    violations: new Violation[] { }
                )
            );
        }
    }
}
