using Auralsys.Audio;
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
            var waveFileUtility = serviceProvider.GetService<IWaveFileUtility>();
            var samplesConverter = serviceProvider.GetService<IAudioSamplesConverter>();

            var inputDevices = deviceManager.GetInputDevices().ToArray();
            Console.WriteLine("\nInput Devices:");
            foreach (var dev in inputDevices)
            {
                Console.WriteLine($"\t{dev.Index}: {dev}");
            }

            int sampleRate = 5512;
            int channels = 1;
            var duration = TimeSpan.FromMilliseconds(10_000);
            var format = new Format(sampleRate, channels, BitDepth.Bit_16);
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
                rec.Start();

                string path = Path.Combine("Recordings", $"device-{rec.Device.Index}.wav");
                                
                //var task = rec.RecordWaveFileAsync(path, duration);
                var task = rec.RecordAsync(duration);

                var spinner = new ConsoleSpinner();
                spinner.UpdateProgress();
                while (!task.IsCompleted)
                {
                    spinner.UpdateProgress();
                    Task.Delay(200).Wait();
                }
                
                var bytes = task.Result;
                waveFileUtility.Write(path, bytes, rec.Format);

                rec.Stop();
            }

            Console.WriteLine();
            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }
    }
}
