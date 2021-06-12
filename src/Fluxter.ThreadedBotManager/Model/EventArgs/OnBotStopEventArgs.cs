namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    public class OnBotStopEventArgs
    {
        public string BotId { get; }

        public OnBotStopEventArgs(string botId)
        {
            this.BotId = botId;
        }
    }
}