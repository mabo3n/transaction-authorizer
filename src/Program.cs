using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Authorizer.Application;
using Authorizer.Interface;

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
                    services
                        .AddMediatR(typeof(Program).Assembly)
                        .AddHostedService<ConsoleService>()
                        .AddScoped<IInputParser<string>, JsonStringInputParser>();
                })
                .RunConsoleAsync();
        }
    }
}
