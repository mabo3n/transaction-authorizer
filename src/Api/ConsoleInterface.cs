using System;
using System.IO;

namespace Authorizer.Api
{
    public class ConsoleInterface : IConsoleInterface
    {
        public Stream GetStdIn()
            => Console.OpenStandardInput();

        public void WriteToStdOut(string text)
            => Console.Out.Write(text);
    }
}
