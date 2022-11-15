using Domain.Interfaces;
using MessageGenerator;
using MessageProcessor;
using MessageStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("----- Host app is starting -----");
            await new HostBuilder()
                  .ConfigureServices((hostContext, services) =>
                  {
                      services.AddLogging();
                      services.AddSingleton<IMessageStorage, SimpleMessageStorage>();
                      services.AddHostedService<MokMessageGenerator>();
                      services.AddHostedService<MokMessageProcessor>();
                  })
                  .ConfigureLogging((hostContext, configLogging) =>
                  {
                      configLogging.AddConsole();
                      configLogging.SetMinimumLevel(LogLevel.Debug);
                  })
                  .RunConsoleAsync();
            Console.WriteLine("----- Host app is stopped -----");
        }
    }
}