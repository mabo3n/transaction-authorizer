using System.Threading;
using System.Threading.Tasks;

namespace Authorizer.Application
{
    public class AuthorizeTransactionHandler
        : IOperationHandler<AuthorizeTransaction>
    {
        public AuthorizeTransactionHandler()
        {

        }

        public Task<OperationResult> Handle(
            AuthorizeTransaction operation,
            CancellationToken cancellationToken
        )
        {
            throw new System.NotImplementedException();
        }
    }
}
