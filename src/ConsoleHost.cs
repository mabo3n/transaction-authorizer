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
        private IMediator Mediator { get; }
        private IInputParser<string> InputParser { get; }

        public ConsoleHost(IMediator mediator, IInputParser<string> inputParser)
        {
            Mediator = mediator;
            InputParser = inputParser;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var stdinReader = new StreamReader(Console.OpenStandardInput()))
            {
                var input = String.Empty;
                while ((input = await stdinReader.ReadLineAsync()) != null)
                {
                    try
                    {
                        var operation = InputParser.Parse(input);
                        var operationResult = await Mediator.Send(operation);
                        Console.WriteLine("Done: " + operationResult.Account.ActiveCard);
                        Console.WriteLine("    : " + operationResult.Account.AvailableLimit);
                        Console.WriteLine("    : " + string.Join(',', operationResult.Violations));
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
