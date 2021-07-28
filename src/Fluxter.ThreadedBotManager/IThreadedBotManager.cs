using System.Threading.Tasks;

namespace Fluxter.ThreadedBotManager
{
    public interface IThreadedBotManager
    {
        Task StopAllAsync();

        Task StopAsync(string botId);

        Task StartAllAsync();
    }
}