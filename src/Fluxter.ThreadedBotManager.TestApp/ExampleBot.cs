namespace Fluxter.ThreadedBotManager.TestApp
{
    using System.Threading.Tasks;
    using Fluxter.ThreadedBotManager.Model;
    using Fluxter.ThreadedBotManager.Model.EventArgs;
    using NLog;

    class ExampleBot : BotBase, IBot
    {
        public string BotId { get; }

        private bool Running { get; set; } = true;

        public ExampleBot(string id)
        {
            this.BotId = id;
        }

        public async Task<int> RunAsync()
        {
            while (this.Running)
            {
                this.Log($"{this.BotId} Running...");
                System.Threading.Thread.Sleep(1000);
            }

            return 0;
        }

        public async Task StopAsync()
        {
            this.Running = false;
        }
    }
}