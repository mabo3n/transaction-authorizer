using MediatR;

namespace Authorizer.Application
{
    public abstract class Operation : IRequest<OperationResult>
    { }
}
