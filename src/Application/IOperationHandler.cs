using MediatR;

namespace Authorizer.Application
{
    public interface IOperationHandler<T> :
        IRequestHandler<T, OperationResult>
        where T : Operation
    { }
}
