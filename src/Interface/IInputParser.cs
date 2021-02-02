using Authorizer.Application;

namespace Authorizer.Interface
{
    public interface IInputParser<TInput>
    {
        Operation Parse(TInput input);
    }
}
