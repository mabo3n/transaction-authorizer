using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Authorizer.Api;
using Authorizer.Domain.Repositories;
using Authorizer.Infrastructure;
using Authorizer.Domain.Services;

namespace Authorizer
{
    public class EntryPoint
    {
        private static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                // .ConfigureLogging(
                //     logging => logging.ClearProviders()
                // )
                .ConfigureServices((hostService, services) => {
                    // Infra
                    services.AddScoped<IDataSource, InMemoryDataSource>();
                    services.AddScoped<IAccountRepository, AccountRepository>();

                    // Application/Domain
                    services.AddScoped<ITransactionRepository, TransactionRepository>();
                    services.AddScoped<ITransactionService, TransactionService>();
                    services.AddMediatR(typeof(EntryPoint).Assembly);

                    // Api
                    services.AddScoped<IConsoleInterface, ConsoleInterface>();
                    services.AddScoped<IInputParser<string>, JsonStringInputParser>();
                    services.AddHostedService<ConsoleHost>();
                })
                .RunConsoleAsync();
        }
    }
}
