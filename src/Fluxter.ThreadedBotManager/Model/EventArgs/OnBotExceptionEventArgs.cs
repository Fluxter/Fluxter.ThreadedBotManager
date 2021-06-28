namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    using System;

    public class OnBotExceptionEventArgs
    {
        public string BotId { get; }

        public Exception Exception { get; }

        public OnBotExceptionEventArgs(string botId, Exception exception)
        {
            this.BotId = botId;
            this.Exception = exception;
        }
    }
}