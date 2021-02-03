using Authorizer.Application;

namespace Authorizer.Api
{
    public interface IInputParser<TInput>
    {
        Operation Parse(TInput input);
    }
}
