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
            var op = new OperationResult(
                account: new Account(payload.AvailableLimit, payload.ActiveCard),
                violations: new Violation[] { }
            );

            return Task.FromResult(op);
        }
    }
}
