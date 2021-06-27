namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    using System.Threading;

    public class BotInstance
    {
        private IBot Bot { get; }

        private Thread Thread { get; set; }

        public BotInstance(IBot bot)
        {
            this.Bot = bot;
        }

        public bool IsRunning => this.Thread.IsAlive;

        public void Start()
        {
            this.Thread = new Thread(() => this.Bot.Run());

            this.Thread.IsBackground = true;
            this.Thread.Start();
        }

        public void Stop()
        {
            this.Bot.Stop();
        }
    }
}