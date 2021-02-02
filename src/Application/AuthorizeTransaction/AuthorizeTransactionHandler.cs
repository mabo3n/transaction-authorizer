using System.Threading;
using System.Threading.Tasks;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Services;
using MediatR;

namespace Authorizer.Application
{
    public class AuthorizeTransactionHandler
        : IRequestHandler<AuthorizeTransaction, OperationResult>
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly ITransactionService transactionService;

        public AuthorizeTransactionHandler(
            ITransactionRepository transactionRepository,
            ITransactionService transactionService
        )
        {
            this.transactionRepository = transactionRepository;
            this.transactionService = transactionService;
        }

        public Task<OperationResult> Handle(
            AuthorizeTransaction payload,
            CancellationToken cancellationToken
        )
        {
            var op = new OperationResult(
                account: new Account(payload.Amount, true),
                violations: new Violation[] { }
            );

            return Task.FromResult(op);
        }
    }
}
