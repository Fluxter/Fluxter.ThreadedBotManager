namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;

    public class BotInstance
    {
        public IBot Bot { get; }

        private Task Task { get; set; }

        public Func<OnBotExceptionEventArgs, Task> BotException { get; set; }

        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public BotInstance(IBot bot)
        {
            this.Bot = bot;
        }

        public bool IsRunning => !this.Task.IsCompleted;

        public Exception Exception => this.Task.Exception;

        public void Start()
        {
            this.Task = new Task(() =>
            {
                var result = this.Bot.RunAsync().Result;
                if (result != 0)
                {
                    var eventArgs = new OnBotExceptionEventArgs(this.Bot.BotId, new Exception("Run didnt return 0. Exit Code: " + result));
                    this.BotException?.Invoke(eventArgs);
                }
            });
            this.Task.ContinueWith(this.ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);

            // this.Task.IsBackground = true;
            this.Task.Start();
        }

        private void ExceptionHandler(Task task)
        {
            this.Logger.Warn($"{this.Bot.BotId} threw an exception: {task.Exception.Message}");
            this.BotException?.Invoke(new OnBotExceptionEventArgs(this.Bot.BotId, task.Exception));
        }

        public async Task Stop()
        {
            await this.Bot.StopAsync();
        }
    }
}