using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Resony.Server
{
    public abstract class AudioServerProcessorBase : IAudioServerProcessor
    {
        private Task _processorTask;

        private readonly ILogger _logger;

        public AudioServerProcessorBase(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetResonyServerLoggerFactory().CreateLogger(this.GetType());
        }
        public abstract Task Process(AudioServerProcessorContext context);

        public async Task Execute(AudioServerProcessorContext context)
        {
            _processorTask?.Dispose();
            _processorTask = Task.Run(async () =>
            {
                try
                {
                    await Process(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Audio Processing component error.");
                }
            });
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _processorTask?.Dispose();
        }
    }
}
