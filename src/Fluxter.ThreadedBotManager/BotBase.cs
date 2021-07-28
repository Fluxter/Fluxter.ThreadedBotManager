namespace Fluxter.ThreadedBotManager
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;

    public abstract class BotBase : IBot
    {
        public string BotId => throw new System.NotImplementedException();

        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Task<int> RunAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public List<string> Logs { get; } = new List<string>();

        public void Log(string text)
        {
            this.Logs.Add($"[{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}]: {text}");
            this.Logger.Debug(text);
        }
    }
}