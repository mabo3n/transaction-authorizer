using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using MediatR;

namespace Authorizer.Application
{
    public abstract class Operation : IRequest<OperationResult> { }

    public class OperationResult
    {
        public Account Account { get; set; }
        public Violation[] Violations { get; set; }
    }
}
