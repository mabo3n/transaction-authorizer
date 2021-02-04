using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Authorizer.Api;
using Authorizer.Application;

namespace Authorizer
{
    public class ConsoleService : IHostedService
    {
        private readonly IConsoleInterface console;
        private readonly JsonStringParser jsonParser;
        private readonly IOperationHandler<CreateAccount> createAccountHandler;
        private readonly IOperationHandler<AuthorizeTransaction> authorizeTransactionHandler;

        public ConsoleService(
            IConsoleInterface console,
            JsonStringParser jsonParser,
            IOperationHandler<CreateAccount> createAccountHandler,
            IOperationHandler<AuthorizeTransaction> authorizeTransactionHandler
        )
        {
            this.console = console;
            this.jsonParser = jsonParser;
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
                        "account" => await createAccountHandler.Handle(jsonParser.Parse<CreateAccount>(payload)),
                        "transaction" => await authorizeTransactionHandler.Handle(jsonParser.Parse<AuthorizeTransaction>(payload)),
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
