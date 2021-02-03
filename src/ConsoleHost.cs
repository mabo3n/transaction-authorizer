using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MediatR;
using Authorizer.Api;

namespace Authorizer
{
    public class ConsoleHost : IHostedService
    {
        private readonly IMediator mediator;
        private readonly IInputParser<string> inputParser;
        private readonly IConsoleInterface console;

        public ConsoleHost(
            IMediator mediator,
            IInputParser<string> inputParser,
            IConsoleInterface console
        )
        {
            this.mediator = mediator;
            this.inputParser = inputParser;
            this.console = console;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var stdin = console.GetStdIn();
            using (var stdinReader = new StreamReader(stdin))
            {
                var input = String.Empty;
                while ((input = await stdinReader.ReadLineAsync()) != null)
                {
                    try
                    {
                        var operation = inputParser.Parse(input);
                        var operationResult = await mediator.Send(operation);
                        console.WriteToStdOut("Done: " + operationResult.Account.ActiveCard);
                        console.WriteToStdOut("    : " + operationResult.Account.AvailableLimit);
                        console.WriteToStdOut("    : " + string.Join(',', operationResult.Violations));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error ocurred: " + ex.ToString());
                        await StopAsync(cancellationToken);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
