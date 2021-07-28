namespace Fluxter.ThreadedBotManager.TestApp
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Fluxter.ThreadedBotManager.Model;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<BotService<ExampleBotManager, ExampleBot>>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}
