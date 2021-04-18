using Resony.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Resony.Server.Processing
{
    internal sealed class ChannelRecordingProcessor : AudioServerProcessorBase
    {
        private readonly IAudioSourceRuntimeManager _audioSourceRuntimeManager;
        private readonly ResonyServerOptions _serverOptions;
        private readonly ILogger _logger;

        #region Ctor
        public ChannelRecordingProcessor(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _audioSourceRuntimeManager = serviceProvider.GetService<IAudioSourceRuntimeManager>();
            _serverOptions = serviceProvider.GetService<ResonyServerOptions>();
            _logger = serviceProvider.GetResonyServerLoggerFactory().CreateLogger<ChannelRecordingProcessor>();
        }

        #endregion
        public override async Task Process(AudioServerProcessorContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            var infos = _audioSourceRuntimeManager.AudioSourceRuntimeInfos;
            List<Task> tasks = new List<Task>();
            foreach (var info in infos)
            {

                var task = Task.Factory.StartNew(async () => await RecordChannel(info, context), TaskCreationOptions.LongRunning);
                tasks.Add(task);                
            }

            Task.WaitAll(tasks.ToArray());
            await Task.CompletedTask;
        }

        private async Task RecordChannel(AudioSourceRuntimeInfo info, AudioServerProcessorContext context)
        {
            try
            {
                int recordings = 0;
                var currentScheduleUtc = DateTime.UtcNow;
                var nextScheduledUtc = DateTime.UtcNow;                
                while (!context.StoppingToken.IsCancellationRequested && _serverOptions.RecordingEnabled && info.AudioSourceDefinition.Enabled)
                {
                    currentScheduleUtc = nextScheduledUtc;
                    nextScheduledUtc = currentScheduleUtc.AddMinutes(_serverOptions.RecordingLengthMinutes);
                    if (recordings == 0 && _serverOptions.RecordingLengthMinutes <= 60 && (60D % _serverOptions.RecordingLengthMinutes == 0))
                    {
                        nextScheduledUtc = currentScheduleUtc.GetNextMinuteSchedule((int)_serverOptions.RecordingLengthMinutes);
                    }

                    var duration = nextScheduledUtc - currentScheduleUtc;
                    using (var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(nextScheduledUtc - DateTime.UtcNow).Token, context.StoppingToken))
                    {                      
                        var task = RecordSchedule(info, currentScheduleUtc, duration, cts.Token);
                        _logger.LogDebug($"Duration: {duration} | Schedule: {currentScheduleUtc} | Time: {DateTime.UtcNow}");

                        try
                        {
                            await Task.Delay(nextScheduledUtc - DateTime.UtcNow, cts.Token);
                        }
                        catch (TaskCanceledException ex)
                        {
                            _logger.LogWarning($"Recording period wait task was cancelled by the supplied cancellation token.", ex);
                        }
                    }

                    recordings++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private async Task<bool> RecordSchedule(AudioSourceRuntimeInfo info, DateTime scheduleUtc, TimeSpan duration, CancellationToken scheduleCancellationToken)
        {
            bool result = false;
            while (!result && !scheduleCancellationToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    string folder = Path.Combine(_serverOptions.RecordingRootFolder, now.ToString("yyyy-MM-dd-HH-mm"));
                    IOHelper.TryCreateDirectory(folder);
                    string path = Path.Combine(folder, $"channel-{info.Recorder.Device.Index.ToString("00")}-{now.ToString("yyyyMMddHHmmss")}.wav");
                    var sw = Stopwatch.StartNew();
                    await info?.Recorder?.RecordWaveFileAsync(path, duration, scheduleCancellationToken);
                    result = true;
                }
                catch (Exception)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_serverOptions.RecorderInvisibiltyTimeoutSeconds), scheduleCancellationToken);
                    }
                    catch (Exception) { }                    
                }
            }

            return result;
        }
    }
}
