using System.Threading;
using System.Threading.Tasks;
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
            throw new System.NotImplementedException();
        }
    }
}
