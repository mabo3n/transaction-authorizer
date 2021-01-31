namespace Authorizer.Application
{
    public abstract class Operation { }

    public interface IOperationHandler<T>
        where T : Operation
    {
        public OperationOutput Handle(T operation);
    }
}
