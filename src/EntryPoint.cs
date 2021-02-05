using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Authorizer.Api;
using Authorizer.Domain.Repositories;
using Authorizer.Infrastructure;
using Authorizer.Domain.Services;
using Authorizer.Application;

namespace Authorizer
{
    public class EntryPoint
    {
        private static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    logging => logging.ClearProviders()
                )
                .ConfigureServices(services => {
                    // Infra
                    services.AddScoped<IDataSource, InMemoryDataSource>();
                    services.AddScoped<IAccountRepository, AccountRepository>();
                    services.AddScoped<ITransactionRepository, TransactionRepository>();

                    // Application/Domain
                    services.AddScoped<ITransactionService, TransactionService>();
                    services.AddScoped<ITransactionService, TransactionService>();
                    services.AddScoped<IOperationHandler<CreateAccount>, CreateAccountHandler>();
                    services.AddScoped<IOperationHandler<AuthorizeTransaction>, AuthorizeTransactionHandler>();

                    // Api
                    services.AddScoped<IConsoleInterface, ConsoleInterface>();
                    services.AddScoped<JsonStringParser>();

                    // Host
                    services.AddHostedService<ConsoleHost>();
                })
                .RunConsoleAsync();
        }
    }
}
