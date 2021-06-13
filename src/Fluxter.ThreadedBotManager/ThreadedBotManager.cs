namespace Fluxter.ThreadedBotManager
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Fluxter.ThreadedBotManager.Model.EventArgs;
    using NLog;

    public abstract class ThreadedBotManager<T> where T : IBot
    {
        private Dictionary<string, Thread> RunningBots { get; } = new Dictionary<string, Thread>();

        public static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

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

        private void ResyncAllBots(object state)
        {
            // Find dead threads
            foreach (var thread in this.RunningBots)
            {
                if (thread.Value.IsAlive)
                {
                    continue;
                }

                this.Logger.Warn($"Thread of bot id {thread.Key} is dead. Stopping it!");
                this.Stop(thread.Key);
            }

            // Start all missing
            this.StartAll();
            this.StopBotsWithPendingStop();
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
            Logger.Info($"Stopping Bot {botId}");
            if (!this.RunningBots.ContainsKey(botId))
            {
                Logger.Warn($"Bot not found {botId}");
                return;
            }

            var thread = this.RunningBots[botId];
            this.BotStop?.Invoke(new OnBotStopEventArgs(botId));
            if (thread.IsAlive)
            {
                thread.Interrupt();
            }

            this.RunningBots.Remove(botId);
        }

        public T Start(string id)
        {
            this.Logger.Info($"Starting Bot {id}");

            var bot = this.GetBotById(id);
            var thread = new Thread(
                new ParameterizedThreadStart(
                    (object bot) => ((T)bot).Run()
                )
            );

            thread.IsBackground = true;
            thread.Start(bot);
            this.RunningBots.Add(id, thread);
            this.BotStart?.Invoke(new OnBotStartEventArgs(bot));

            return bot;
        }

        public void StartAll()
        {
            this.Logger.Info($"Starting all bots...");

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
