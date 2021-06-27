namespace Fluxter.ThreadedBotManager
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Fluxter.ThreadedBotManager.Model.EventArgs;
    using NLog;

    public abstract class ThreadedBotManager<T> where T : IBot
    {
        private Dictionary<string, BotInstance> RunningBots { get; } = new Dictionary<string, BotInstance>();

        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public Timer ResyncAllBotsTimer { get; }

        public Action<OnBotStopEventArgs> BotStop;

        public Action<OnBotStopEventArgs> BotPendingStopResolved;

        public Action<OnBotStartEventArgs> BotStart;

        public ThreadedBotManager()
        {
            this.ResyncAllBotsTimer = new Timer(new TimerCallback(this.ResyncAllBots), null, 10000, 10000);
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

        private void ResyncAllBots(object state)
        {
            // Find dead threads
            foreach (var thread in this.RunningBots)
            {
                if (thread.Value.IsRunning)
                {
                    continue;
                }

                this.Logger.Warn($"Thread of bot id {thread.Key} is dead. Stopping it!");
                this.Stop(thread.Key);
            }

            // Start all missing
            this.StartAll();
            this.StopBotsWithPendingStop();

            foreach (var bot in this.RunningBots)
            {
                this.SendHeartbeat(bot.Key);
            }
        }

        public void StopAll()
        {
            this.Logger.Info($"Stopping all Bots...");
            foreach (var thread in this.RunningBots)
            {
                this.Logger.Debug($" - Stopping bot {thread.Key}...");
                this.Stop(thread.Key);
            }
        }

        public void Stop(string botId)
        {
            this.Logger.Info($"Stopping Bot {botId}");
            if (!this.RunningBots.ContainsKey(botId))
            {
                this.Logger.Warn($"Bot not found {botId}");
                return;
            }

            var instance = this.RunningBots[botId];
            instance.Stop();
            while (instance.IsRunning)
            {
                this.Logger.Info($"Still Stopping Bot {botId}...");
                Thread.Sleep(500);
            }

            this.BotStop?.Invoke(new OnBotStopEventArgs(botId));
            this.RunningBots.Remove(botId);
        }

        public T Start(string id)
        {
            this.Logger.Info($"Starting Bot {id}");

            var bot = this.GetBotById(id);
            var instance = new BotInstance(bot);
            instance.Start();

            this.RunningBots.Add(id, instance);
            this.BotStart?.Invoke(new OnBotStartEventArgs(bot));

            return bot;
        }

        public void StartAll()
        {
            this.Logger.Debug($"Starting all bots...");

            foreach (var botId in this.GetAllBotIds())
            {
                if (this.RunningBots.ContainsKey(botId))
                {
                    continue;
                }
                this.Start(botId);
            }
        }

        public void StopBotsWithPendingStop()
        {
            this.Logger.Debug($"Stopping all Bots with pending stop...");
            foreach (var botId in this.GetBotIdsToStop())
            {
                this.Logger.Info($"Stopping Bot {botId}");

                if (this.RunningBots.ContainsKey(botId))
                {
                    this.Stop(botId);
                }

                this.BotPendingStopResolved?.Invoke(new OnBotStopEventArgs(botId));
            }
        }
    }
}
