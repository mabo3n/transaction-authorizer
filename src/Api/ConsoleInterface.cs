using System;
using System.IO;

namespace Authorizer.Api
{
    public class ConsoleInterface : IConsoleInterface
    {
        public Stream Stdin => Console.OpenStandardInput();

        public Stream Stdout => Console.OpenStandardOutput();

        public Stream Stderr => Console.OpenStandardError();
    }
}
