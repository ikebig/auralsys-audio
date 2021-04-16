using Resony.Server;
using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Sample.AudioServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            
            var audioServer = DependencyHelper.ServiceProvider.GetService<IAudioServer>();

            var spinner = new ConsoleSpinner();
            audioServer.Start(cts.Token);
            while (!cts.IsCancellationRequested)
            {
                spinner.UpdateProgress();
                Task.Delay(200).Wait();
            }            
            audioServer.Stop();
            Task.Delay(2000).Wait();
            Console.WriteLine();
            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }
    }
}
