using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Authorizer.Api;
using Authorizer.Application;

namespace Authorizer
{
    public class ConsoleHost : IHostedService
    {
        private readonly JsonStringParser jsonParser;
        private readonly IOperationHandler<CreateAccount> createAccountHandler;
        private readonly IOperationHandler<AuthorizeTransaction> authorizeTransactionHandler;

        public ConsoleHost(
            JsonStringParser jsonParser,
            IOperationHandler<CreateAccount> createAccountHandler,
            IOperationHandler<AuthorizeTransaction> authorizeTransactionHandler
        )
        {
            this.jsonParser = jsonParser;
            this.createAccountHandler = createAccountHandler;
            this.authorizeTransactionHandler = authorizeTransactionHandler;
        }

        T As<T>(string payload) => jsonParser.Parse<T>(payload);

        private async Task<string> Resolve(string input)
        {
            var (operationName, payload) = jsonParser.GetRootAttribute(input);

            var operationResult = operationName switch
            {
                "account" => await createAccountHandler.Handle(
                    As<CreateAccount>(payload)
                ),
                "transaction" => await authorizeTransactionHandler.Handle(
                    As<AuthorizeTransaction>(payload)
                ),
                _ => throw new InvalidOperationException(
                    $"Unrecognizable operation \"{operationName}\""
                ),
            };

            return jsonParser.Stringify<OperationResult>(operationResult);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var input = String.Empty;
                while ((input = Console.ReadLine()) != null)
                {
                    var output = await Resolve(input);

                    Console.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error ocurred: {ex.ToString()}");
            }
            finally
            {
                await StopAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
