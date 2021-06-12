# Fluxter.ThreadedBotManager
The ThreadedBotManager is used to start and stop bots.

It will automatically spawn threads for each bot

## How to use it
Create a Bot Class
```csharp
namespace YourApp.Model
{
    using Fluxter.ThreadedBotManager;

    class MyBot : IBot
    {
        public string BotId { get; } 

        public void Run()
        {
            // Run your bot endlessly
        }
    }
}
```
Create a basic BotManager and extend from ThreadedBotManager like so
```csharp
namespace YourApp
{
    using Fluxter.ThreadedBotManager;
    using System.Collections.Generic;
    using Fluxter.ThreadedBotManager;
    using Fluxter.ThreadedBotManager.Model.EventArgs;

    class MyBotManager : ThreadedBotManager<MyBot>
    {
        public BotManager()
        {
            this.BotPendingStopResolved += this.OnBotPendingStopResolved;
            this.BotStop += this.OnBotStop;
            this.BotStart += this.OnBotStart;
        }

        protected override T GetBotById(string botId)
        {
            throw new Exception("Please overwrite this method!");
        }

        protected override IEnumerable<string> GetAllBotIds()
        {
            throw new Exception("Please overwrite this method!");
        }

        protected override IEnumerable<string> GetBotIdsToStop()
        {
            throw new Exception("Please overwrite this method!");
        }

        private void OnBotStart(OnBotStartEventArgs args)
        {
        }

        private void OnBotStop(OnBotStopEventArgs args)
        {
        }
        
        private void OnBotPendingStopResolved(OnBotStopEventArgs args)
        {
        }
    }
}
```

## Logging
The Bot Manager uses Nlog to log everything