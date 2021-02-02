using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Authorizer.Interface;
using Authorizer.Domain.Repositories;
using Authorizer.Infrastructure;
using Authorizer.Domain.Services;

namespace Authorizer
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                // .ConfigureLogging(
                //     logging => logging.ClearProviders()
                // )
                .ConfigureServices((hostService, services) => {
                    services.AddHostedService<ConsoleService>();

                    services.AddMediatR(typeof(Program).Assembly);
                    services.AddScoped<IInputParser<string>, JsonStringInputParser>();
                    services.AddScoped<ITransactionRepository, TransactionRepository>();
                    services.AddScoped<ITransactionService, TransactionService>();
                })
                .RunConsoleAsync();
        }
    }
}
