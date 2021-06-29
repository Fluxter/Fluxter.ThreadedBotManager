namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    using System.Threading.Tasks;
    
    public interface IBot
    {
        string BotId { get; }

        Task<int> RunAsync();

        void Stop();
    }
}