using Microsoft.Extensions.DependencyInjection;
using Resony;
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
            var duration = TimeSpan.FromMilliseconds(12_000);
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

                string pathDisk = Path.Combine("Recordings", $"device-{rec.Device.Index}-record-to-disk.wav");
                string pathMemory = Path.Combine("Recordings", $"device-{rec.Device.Index}-record-to-memory.wav");

                var taskDisk = rec.RecordWaveFileAsync(pathDisk, duration);
                var taskMemory = rec.RecordAsync(duration);

                var spinner = new ConsoleSpinner();
                spinner.UpdateProgress();
                while (!taskDisk.IsCompleted && !taskMemory.IsCompleted)
                {
                    spinner.UpdateProgress();
                    Task.Delay(200).Wait();
                }

                var bytes = taskMemory.Result;
                waveFileUtility.Write(pathMemory, bytes, rec.Format);

                rec.Stop();
            }

            Console.WriteLine();
            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }
    }
}
