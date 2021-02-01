using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Authorizer
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => {
                    logging
                        .ClearProviders();
                })
                .ConfigureServices((hostContext, services) => {
                    services
                        .AddHostedService<ConsoleService>();
                })
                .RunConsoleAsync();
        }
    }
}
