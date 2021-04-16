using System;
using System.Threading;
using System.Threading.Tasks;

namespace Resony.Server
{
    public interface IAudioServerProcessor : IDisposable
    {
        Task Execute(AudioServerProcessorContext context);
    }

    public class AudioServerProcessorContext
    {
        public AudioServerProcessorContext(CancellationToken stoppingToken)
        {
            StoppingToken = stoppingToken;
        }

        public CancellationToken StoppingToken { get; } = default;
    }
}
