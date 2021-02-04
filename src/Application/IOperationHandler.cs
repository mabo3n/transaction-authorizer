using System.Threading.Tasks;

namespace Authorizer.Application
{
    public interface IOperationHandler<TOperation>
        where TOperation: Operation
    {
        public Task<OperationResult> Handle(TOperation operation);
    }
}
