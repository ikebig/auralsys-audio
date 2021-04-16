using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Resony.Server.Processing
{
    internal sealed class AudioRecorderInvisibitilityProcessor : AudioServerProcessorBase
    {
        private readonly IAudioSourceRuntimeManager _audioSourceRuntimeManager;
        private readonly ResonyServerOptions _options;
        private readonly ILogger _logger;
        public AudioRecorderInvisibitilityProcessor(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _audioSourceRuntimeManager = serviceProvider.GetService<IAudioSourceRuntimeManager>();
            _options = serviceProvider.GetService<ResonyServerOptions>();
            _logger = serviceProvider.GetResonyServerLoggerFactory().CreateLogger<AudioRecorderInvisibitilityProcessor>();
        }

        public override async Task Process(AudioServerProcessorContext context)
        {
            try
            {
                while (!context.StoppingToken.IsCancellationRequested)
                {
                    var runtimeInfos = _audioSourceRuntimeManager.AudioSourceRuntimeInfos;
                    foreach (var info in runtimeInfos)
                    {
                        if (info == null || info.AudioSourceDefinition == null || info.AudioSourceDefinition.Source.IsEmpty() || info.AudioSourceDefinition.Name.IsEmpty())
                        {
                            continue;
                        }

                        if (info.LastHeartBeatUtc.AddSeconds(_options.RecorderInvisibiltyTimeoutSeconds) > DateTime.UtcNow)
                        {
                            continue;
                        }

                        try
                        {
                            if (!context.StoppingToken.IsCancellationRequested)
                            {
                                await _audioSourceRuntimeManager.ReloadRecorder(info.AudioSourceDefinition.Name, context.StoppingToken);
                            }
                        }
                        catch (Exception ex) 
                        {
                            _logger.LogError(ex, $"Error reloading recorder '{info?.AudioSourceDefinition?.Name}'.");
                        }
                    }

                    if (!context.StoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_options.RecorderInvisibiltyTimeoutSeconds));
                    }
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Processing error.");
            }            
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
