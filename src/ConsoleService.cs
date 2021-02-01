using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Authorizer
{
    public class ConsoleService : IHostedService
    {
        private ILogger<ConsoleService> Logger { get; }

        public ConsoleService(ILogger<ConsoleService> logger)
        {
            Logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var stdinReader = new StreamReader(Console.OpenStandardInput()))
            {
                var input = String.Empty;
                while ((input = await stdinReader.ReadLineAsync()) != null)
                {
                    Console.WriteLine($"Input: {input}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
