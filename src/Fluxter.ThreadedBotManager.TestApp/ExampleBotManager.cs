using System.Collections.Generic;
using NLog;

namespace Fluxter.ThreadedBotManager.TestApp
{
    class ExampleBotManager : ThreadedBotManager<ExampleBot>, IThreadedBotManager
    {
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        protected override IEnumerable<string> GetAllBotIds()
        {
            return new List<string>
            {
                "start-and-run-always",
                "start-and-stop"
            };
        }

        protected override ExampleBot GetBotById(string botId)
        {
            return new ExampleBot(botId);
        }

        protected override IEnumerable<string> GetBotIdsToStop()
        {
            return new List<string>
            {
                "start-and-stop"
            };
        }

        protected override void SendHeartbeat(string botId)
        {
            this.Logger.Info($"{botId}: Sending heartbeat...");
        }
    }
}