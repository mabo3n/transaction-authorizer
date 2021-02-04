using System.IO;

namespace Authorizer.Api
{
    public interface IConsoleInterface
    {
        Stream Stdin { get; }
        Stream Stdout { get; }
        Stream Stderr { get; }
    }
}
