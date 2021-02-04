using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MediatR;
using Authorizer.Api;
using Authorizer.Application;

namespace Authorizer
{
    public class ConsoleHost : IHostedService
    {
        private readonly IConsoleInterface console;
        private readonly JsonStringParser jsonParser;
        private readonly IMediator mediator;
        private readonly CreateAccountHandler createAccountHandler;
        private readonly AuthorizeTransactionHandler authorizeTransactionHandler;

        public ConsoleHost(
            IConsoleInterface console,
            JsonStringParser jsonParser,
            IMediator mediator,
            CreateAccountHandler createAccountHandler,
            AuthorizeTransactionHandler authorizeTransactionHandler
        )
        {
            this.console = console;
            this.jsonParser = jsonParser;
            this.mediator = mediator;
            this.createAccountHandler = createAccountHandler;
            this.authorizeTransactionHandler = authorizeTransactionHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var stdin = console.GetStdIn();
            using var stdinReader = new StreamReader(stdin);
            var input = String.Empty;

            while ((input = await stdinReader.ReadLineAsync()) != null)
            {
                try
                {
                    var (operationName, payload)
                        = jsonParser.GetRootAttribute(input);

                    var result = operationName switch
                    {
                        "account" => await createAccountHandler.Handle(jsonParser.Parse<CreateAccount>(payload), CancellationToken.None),
                        "transaction" => await authorizeTransactionHandler.Handle(jsonParser.Parse<AuthorizeTransaction>(payload), CancellationToken.None),
                        _ => throw new Exception(),
                    };

                    console.WriteToStdOut(jsonParser.Stringify<OperationResult>(result));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error ocurred: " + ex.ToString());
                    await StopAsync(cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
