namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    public class OnBotStartEventArgs
    {
        public IBot Bot { get; }

        public OnBotStartEventArgs(IBot bot)
        {
            this.Bot = bot;
        }
    }
}