

namespace Fluxter.ThreadedBotManager
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public class BotService<TManager, TBot> : IHostedService, IDisposable
        where TBot : IBot
        where TManager : ThreadedBotManager<TBot>, new()
    {
        private TManager BotManager { get; }

        public BotService()
        {
            this.BotManager = new TManager();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.BotManager.StartAllAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.BotManager.StopAllAsync().Wait();
        }
    }
}