using System;
using Take.Blip.Client.Console;

namespace event_bot
{
    class Program
    {
        static int Main(string[] args) => ConsoleRunner.RunAsync(args).GetAwaiter().GetResult();
    }
}