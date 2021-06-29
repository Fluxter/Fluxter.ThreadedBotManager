namespace Fluxter.ThreadedBotManager.Model.EventArgs
{
    using System;
    using System.Threading.Tasks;

    public class BotInstance
    {
        private IBot Bot { get; }

        private Task Task { get; set; }

        public Action<OnBotExceptionEventArgs> BotException { get; set; }

        public BotInstance(IBot bot)
        {
            this.Bot = bot;
        }

        public bool IsRunning => !this.Task.IsCompleted;

        public Exception Exception => this.Task.Exception;

        public void Start()
        {
            this.Task = new Task(async () =>
            {
                var result = await this.Bot.RunAsync();
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
            this.BotException?.Invoke(new OnBotExceptionEventArgs(this.Bot.BotId, task.Exception));
        }

        public void Stop()
        {
            this.Bot.Stop();
        }
    }
}