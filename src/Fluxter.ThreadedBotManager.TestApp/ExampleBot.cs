using Fluxter.ThreadedBotManager.Model.EventArgs;
using NLog;

namespace Fluxter.ThreadedBotManager.TestApp
{
    class ExampleBot : IBot
    {
        public string BotId { get; }

        private bool Running { get; set; } = true;

        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public ExampleBot(string id)
        {
            this.BotId = id;
        }

        public void Run()
        {
            while (this.Running)
            {
                this.Logger.Debug($"{this.BotId} Running...");
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            this.Running = false;
        }
    }
}