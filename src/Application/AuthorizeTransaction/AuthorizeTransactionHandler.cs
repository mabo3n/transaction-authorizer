using System.Threading;
using System.Threading.Tasks;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using MediatR;

namespace Authorizer.Application
{
    public class AuthorizeTransactionHandler
        : IRequestHandler<AuthorizeTransaction, OperationResult>
    {
        public Task<OperationResult> Handle(
            AuthorizeTransaction payload,
            CancellationToken cancellationToken
        )
        {
            var op = new OperationResult() {
                Account = new Account(payload.Amount, true),
                Violations = new Violation[] { }
            };

            return Task.FromResult(op);
        }
    }
}
