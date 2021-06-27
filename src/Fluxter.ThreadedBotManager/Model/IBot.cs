namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    public interface IBot
    {
        string BotId { get; }

        void Run();

        void Stop();
    }
}