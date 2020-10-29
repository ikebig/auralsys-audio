using Auralsys.Audio;
using Auralsys.Audio.ManagedBass.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Recording
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = DependencyHelper.ServiceProvider;
            var deviceManager = serviceProvider.GetService<IDeviceManager>();
            var recorderFactory = serviceProvider.GetService<IRecorderFactory>();

            var inputDevices = deviceManager.GetInputDevices().ToArray();
            Console.WriteLine("\nInput Devices:");
            foreach (var dev in inputDevices)
            {
                Console.WriteLine($"\t{dev.Index}: {dev}");
            }

            int sampleRate = 41_000;
            int channels = 1;
            var duration = TimeSpan.FromMilliseconds(10_000);
            var format = new Format(sampleRate, channels);
            Console.WriteLine("\nEnter input device number:");
            int deviceIndex = -1;

            while (!int.TryParse(Console.ReadLine(), out deviceIndex) || !inputDevices.Any(x => x.Index == deviceIndex))
            {
                Console.WriteLine("Please enter a valid input device number:");
            }
            Console.WriteLine($"{inputDevices.FirstOrDefault(x => x.Index == deviceIndex)}.");
            Console.WriteLine();

            using (var rec = recorderFactory.Create(inputDevices[deviceIndex], format))
            {
                string path = Path.Combine("Recordings", $"device-{rec.Device.Index}.wav");

                rec.Start();
                var task = rec.RecordWaveFileAsync(path, duration);

                var spinner = new ConsoleSpinner();
                spinner.UpdateProgress();
                while (!task.IsCompleted)
                {
                    spinner.UpdateProgress();
                    Task.Delay(200).Wait();
                }

                rec.Stop();
            }

            Console.WriteLine();
            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }
    }
}
