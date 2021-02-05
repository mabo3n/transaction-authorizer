using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Authorizer.Api;
using Authorizer.Application;

namespace Authorizer
{
    public class ConsoleHost : IHostedService
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


        private async Task<string> Resolve(string input)
        {
            T As<T>(string payload) => jsonParser.Parse<T>(payload);
            var (operationName, payload) = jsonParser.GetRootAttribute(input);

            var operationResult = operationName switch
            {
                "account" => await createAccountHandler.Handle(
                    As<CreateAccount>(payload)
                ),
                "transaction" => await authorizeTransactionHandler.Handle(
                    As<AuthorizeTransaction>(payload)
                ),
                _ => throw new Exception(),
            };

            return jsonParser.Stringify<OperationResult>(operationResult);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var stdinReader = new StreamReader(console.Stdin);
            using var stdoutWriter = new StreamWriter(console.Stdout);
            using var stderrWriter = new StreamWriter(console.Stderr);

            try
            {
                var input = String.Empty;
                while ((input = await stdinReader.ReadLineAsync()) != null)
                {
                    var output = await Resolve(input);

                    stdoutWriter.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                stderrWriter.WriteLine($"An error ocurred: {ex.ToString()}");
                await StopAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
