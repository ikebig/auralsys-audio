using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Resony.Server
{
    internal class AudioSourceRuntimeManager : IAudioSourceRuntimeManager
    {
        #region Fields

        private readonly Dictionary<string, AudioSourceRuntimeInfo> _audioSourceRuntimeInfos = new Dictionary<string, AudioSourceRuntimeInfo>(StringComparer.OrdinalIgnoreCase);

        private readonly IServiceProvider _serviceProvider;
        private readonly ResonyServerOptions _options;
        private readonly IRecorderFactory _recorderFactory;
        private readonly IDeviceManager _deviceManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public AudioSourceRuntimeManager(IServiceProvider serviceProvider)
            : this(serviceProvider,
                 serviceProvider.GetService<ResonyServerOptions>(),
                 serviceProvider.GetService<IRecorderFactory>(),
                 serviceProvider.GetService<IDeviceManager>(),
                 serviceProvider.GetResonyServerLoggerFactory())
        {
        }

        internal AudioSourceRuntimeManager(
            IServiceProvider serviceProvider,
            ResonyServerOptions options,
            IRecorderFactory recorderFactory,
            IDeviceManager deviceManager,
            ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _recorderFactory = recorderFactory;
            _deviceManager = deviceManager;
            _logger = loggerFactory.CreateLogger<AudioSourceRuntimeManager>();
        }

        #endregion

        #region Utils

        private async Task Kill(string audioSourceDefinitionName)
        {
            if (audioSourceDefinitionName.IsEmpty() || !_audioSourceRuntimeInfos.ContainsKey(audioSourceDefinitionName))
            {
                return;
            }

            if (_audioSourceRuntimeInfos.TryGetValue(audioSourceDefinitionName, out var runtimeInfo) && runtimeInfo != null)
            {
                try
                {
                    runtimeInfo.Recorder?.Dispose();
                }
                catch (Exception) { }
            }

            await Task.CompletedTask;
        }
        private async Task KillAll()
        {
            if (_audioSourceRuntimeInfos == null || !_audioSourceRuntimeInfos.Any())
            {
                return;
            }

            foreach (var key in _audioSourceRuntimeInfos.Keys)
            {
                await Kill(key);
            }
        }

        #endregion

        #region Methods

        public IReadOnlyCollection<AudioSourceRuntimeInfo> AudioSourceRuntimeInfos => _audioSourceRuntimeInfos.Values.ToArray() ?? new AudioSourceRuntimeInfo[0];
        public async Task<bool> ReloadRecorder(string audioSourceDefinitionName, CancellationToken cancellationToken = default)
        {
            bool result = false;

            if (cancellationToken.IsCancellationRequested)
            {
                return result;
            }

            if (audioSourceDefinitionName.IsEmpty() || !_audioSourceRuntimeInfos.ContainsKey(audioSourceDefinitionName))
            {
                return result;
            }

            if (!_audioSourceRuntimeInfos.TryGetValue(audioSourceDefinitionName, out var runtimeInfo) || runtimeInfo == null || runtimeInfo.AudioSourceDefinition == null)
            {
                return result;
            }

            if (!runtimeInfo.AudioSourceDefinition.Enabled || runtimeInfo.AudioSourceDefinition.Source.IsEmpty())
            {
                return result;
            }
           
            try
            {
                await Kill(audioSourceDefinitionName);

                int[] validChannels = { 1, 2 };
                int channels = validChannels.Contains(_options.RecordingChannels) ? _options.RecordingChannels : Constants.Audio.Formats.DefaultChannels;
                int sampleRate = _options.RecordingSampleRate > 0 ? _options.RecordingSampleRate : Constants.Audio.Formats.DefaultSampleRate;
                BitDepth bitDepth = _options.RecordingBitDepth;

                var format = new Format(sampleRate: sampleRate, channels: channels, bitDepth: bitDepth);
                var device = _deviceManager.GetInputDevice(runtimeInfo.AudioSourceDefinition.Source);
                var recorder = _recorderFactory.Create(device, format);
                runtimeInfo.UpdateRecorder(recorder);
                if (!cancellationToken.IsCancellationRequested && recorder != null && runtimeInfo.Recorder.Start())
                {
                    runtimeInfo.HeartBeat(true);
                    runtimeInfo.Recorder.DataAvailable += (args) =>
                    {
                        runtimeInfo.HeartBeat();
                    };

                    result = true;
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error loading recorder.");
            }

            return result;
        }
        public async Task<int> LoadRuntimeInfos(CancellationToken cancellationToken = default)
        {
            int result = 0;

            if (cancellationToken.IsCancellationRequested)
            {
                return result;
            }

            try
            {
                await UnloadRuntimeInfos();

                _logger.LogDebug("Loading audio source runtime infos...");
                var audioSourceDefinitions = _options.AudioSourceDefinitionFactory(_serviceProvider).ToList();

                for (int i = 0; i < audioSourceDefinitions.Count; i++)
                {
                    var runtimeInfo = new AudioSourceRuntimeInfo(audioSourceDefinitions[i], null);
                    if (!cancellationToken.IsCancellationRequested && _audioSourceRuntimeInfos.TryAdd(audioSourceDefinitions[i].Name, runtimeInfo))
                    {
                        await ReloadRecorder(runtimeInfo.AudioSourceDefinition.Name, cancellationToken);
                        result++;
                    }
                }

                _logger.LogDebug($"{result} audio source runtime infos loaded sucessfully.");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occured while loading audio source runtime infos.");
            }

            return result;
        }
        public async Task UnloadRuntimeInfos()
        {
            try
            {
                _logger.LogDebug("Unloading audio source runtime infos...");
                await KillAll();
                _audioSourceRuntimeInfos.Clear();
                _logger.LogDebug("Audio source runtime infos unloaded sucessfully.");
                await Task.CompletedTask;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occured while unloading audio source runtime infos.");
            }
        }

        #endregion
    }
}
