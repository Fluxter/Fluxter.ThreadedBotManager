namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    public interface IBot
    {
        string BotId { get; }

        Task<int> RunAsync();

        void Stop();
    }
}