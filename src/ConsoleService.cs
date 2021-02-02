using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Authorizer.Application;
using Microsoft.Extensions.Hosting;
using MediatR;

namespace Authorizer
{
    public class ConsoleService : IHostedService
    {
        private IMediator Mediator { get; }

        public ConsoleService(IMediator mediator)
        {
            Mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var stdinReader = new StreamReader(Console.OpenStandardInput()))
            {
                var input = String.Empty;
                while ((input = await stdinReader.ReadLineAsync()) != null)
                {
                    Console.WriteLine($"Processing: {input}");
                    try
                    {
                        var result = await Mediator.Send(new CreateAccount());
                        Console.WriteLine("Done: " + result.Account.AvailableLimit);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error ocurred: {ex.ToString()}");
                        await StopAsync(cancellationToken);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
