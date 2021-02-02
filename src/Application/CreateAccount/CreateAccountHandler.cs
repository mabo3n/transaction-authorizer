using System.Threading;
using System.Threading.Tasks;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using MediatR;

namespace Authorizer.Application
{
    public class CreateAccountHandler
        : IRequestHandler<CreateAccount, OperationResult>
    {
        public Task<OperationResult> Handle(
            CreateAccount payload,
            CancellationToken cancellationToken
        )
        {
            var op = new OperationResult() {
                Account = new Account(payload.AvailableLimit, payload.ActiveCard),
                Violations = new Violation[] { },
            };

            return Task.FromResult(op);
        }
    }
}
