using System.Threading;
using System.Threading.Tasks;

namespace Authorizer.Application
{
    public class CreateAccountHandler
        : IOperationHandler<AuthorizeTransaction>
    {
        public CreateAccountHandler()
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
