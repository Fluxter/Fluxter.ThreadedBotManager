namespace Fluxter.ThreadedBotManager.TestApp
{
    using System.Threading.Tasks;
    using Fluxter.ThreadedBotManager.Model.EventArgs;
    using NLog;

    class ExampleBot : IBot
    {
        public string BotId { get; }

        private bool Running { get; set; } = true;

        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public ExampleBot(string id)
        {
            this.BotId = id;
        }

        public async Task<int> RunAsync()
        {
            while (this.Running)
            {
                this.Logger.Debug($"{this.BotId} Running...");
                System.Threading.Thread.Sleep(1000);
            }

            return 0;
        }

        public void Stop()
        {
            this.Running = false;
        }
    }
}