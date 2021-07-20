namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class BotBase : IBot
    {
        public string BotId => throw new System.NotImplementedException();

        public Task<int> RunAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public List<string> Logs { get; } = new List<string>();

        protected void AddLog(string text)
        {
            this.Logs.Add(text);
        }
    }
}