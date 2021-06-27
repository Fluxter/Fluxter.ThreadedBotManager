using System;

namespace Fluxter.ThreadedBotManager.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var botManager = new ExampleBotManager();
            botManager.StartAll();

            Console.ReadLine();
        }
    }
}
