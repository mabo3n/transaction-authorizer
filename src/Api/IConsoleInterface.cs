using System.IO;

namespace Authorizer.Api
{
    public interface IConsoleInterface
    {
        Stream GetStdIn();
        void WriteToStdOut(string text);
    }
}
