using System.Threading;
using System.Threading.Tasks;
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
                Account = new Authorizer.Domain.Entities.Account(123, true)
            };

            return Task.FromResult(op);
        }
    }
}
