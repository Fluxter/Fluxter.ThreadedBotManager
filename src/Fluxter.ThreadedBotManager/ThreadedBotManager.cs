namespace Fluxter.ThreadedBotManager
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fluxter.ThreadedBotManager.Model;
    using Fluxter.ThreadedBotManager.Model.EventArgs;
    using NLog;

    public abstract class ThreadedBotManager<T> : IThreadedBotManager where T : IBot
    {
        private Dictionary<string, BotInstance> RunningBots { get; } = new Dictionary<string, BotInstance>();

        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public Timer ResyncAllBotsTimer { get; }

        public Action<OnBotStopEventArgs> BotStop;

        public Action<OnBotStopEventArgs> BotPendingStopResolved;

        public Action<OnBotStartEventArgs> BotStart;

        public Action<OnBotExceptionEventArgs> BotException;

        public ThreadedBotManager()
        {
            this.ResyncAllBotsTimer = new Timer(new TimerCallback(this.ResyncAllBotsAsync), null, 10000, 10000);
        }

        protected virtual T GetBotById(string botId)
        {
            throw new Exception("Please overwrite this method!");
        }

        protected virtual IEnumerable<string> GetAllBotIds()
        {
            throw new Exception("Please overwrite this method!");
        }

        protected virtual IEnumerable<string> GetBotIdsToStop()
        {
            throw new Exception("Please overwrite this method!");
        }

        protected virtual void SendHeartbeat(string botId)
        {
        }

        protected virtual void UpdateLog(string botId, List<string> logs)
        {
        }

        private async void ResyncAllBotsAsync(object state)
        {
            // Find dead threads
            foreach (var thread in this.RunningBots)
            {
                if (thread.Value.IsRunning)
                {
                    this.UpdateLog(thread.Key, thread.Value.Bot.Logs);
                    continue;
                }

                this.Logger.Warn($"Thread of bot id {thread.Key} is dead. Stopping it!");
                if (thread.Value.Exception != null)
                {
                    this.Logger.Warn("Thread has been shutdown due to an error: " + thread.Value.Exception.Message);
                    this.BotException?.Invoke(new OnBotExceptionEventArgs(thread.Key, thread.Value.Exception));
                }
                await this.StopAsync(thread.Key);
            }

            // Start all missing
            await this.StartAllAsync();
            await this.StopBotsWithPendingStopAsync();

            foreach (var bot in this.RunningBots)
            {
                this.SendHeartbeat(bot.Key);
            }
        }

        public async Task StopAllAsync()
        {
            this.Logger.Info($"Stopping all Bots...");
            foreach (var thread in this.RunningBots)
            {
                this.Logger.Debug($" - Stopping bot {thread.Key}...");
                await this.StopAsync(thread.Key);
            }
        }

        public async Task StopAsync(string botId)
        {
            this.Logger.Info($"Stopping Bot {botId}");
            if (!this.RunningBots.ContainsKey(botId))
            {
                this.Logger.Warn($"Bot not found {botId}");
                return;
            }

            var instance = this.RunningBots[botId];
            await instance.Stop();
            instance.BotException -= this.OnBotInstanceException;
            while (instance.IsRunning)
            {
                this.Logger.Info($"Still Stopping Bot {botId}...");
                Thread.Sleep(500);
            }

            this.BotStop?.Invoke(new OnBotStopEventArgs(botId));
            this.RunningBots.Remove(botId);
        }

        public async Task<T> StartAsync(string id)
        {
            this.Logger.Info($"Starting Bot {id}");

            var bot = this.GetBotById(id);
            var instance = new BotInstance(bot);
            instance.BotException += this.OnBotInstanceException;
            instance.Start();

            this.RunningBots.Add(id, instance);
            this.BotStart?.Invoke(new OnBotStartEventArgs(bot));

            return bot;
        }

        private async Task OnBotInstanceException(OnBotExceptionEventArgs eventArgs)
        {
            this.BotException?.Invoke(new OnBotExceptionEventArgs(eventArgs.BotId, eventArgs.Exception));
            await this.StopAsync(eventArgs.BotId);
        }

        public async Task StartAllAsync()
        {
            this.Logger.Debug($"Starting all bots...");

            foreach (var botId in this.GetAllBotIds())
            {
                if (this.RunningBots.ContainsKey(botId))
                {
                    continue;
                }
                await this.StartAsync(botId);
            }
        }

        public async Task StopBotsWithPendingStopAsync()
        {
            this.Logger.Debug($"Stopping all Bots with pending stop...");
            foreach (var botId in this.GetBotIdsToStop())
            {
                this.Logger.Info($"Stopping Bot {botId}");

                if (this.RunningBots.ContainsKey(botId))
                {
                    await this.StopAsync(botId);
                }

                this.BotPendingStopResolved?.Invoke(new OnBotStopEventArgs(botId));
            }
        }
    }
}
