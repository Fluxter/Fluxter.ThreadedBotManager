namespace Fluxter.ThreadedBotManager
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBot
    {
        string BotId { get; }

        List<string> Logs { get; }

        Task<int> RunAsync();

        Task StopAsync();
    }
}