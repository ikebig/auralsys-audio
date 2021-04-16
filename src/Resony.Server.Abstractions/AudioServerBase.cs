using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Resony.Server
{
    public abstract class AudioServerBase : IAudioServer
    {
        #region Fields

        private CancellationTokenSource _stoppingTokenSource;
        private CancellationTokenRegistration _stoppingTokenRegistration;
        private bool _stopping;
        private object _stoppingLock = nameof(_stoppingLock);

        private readonly IAudioSourceRuntimeManager _audioSourceRuntimeManager;
        private readonly IEnumerable<IAudioServerProcessor> _audioServerProcessors;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public AudioServerBase(IServiceProvider serviceProvider)
            :this(serviceProvider.GetService<IAudioSourceRuntimeManager>(),
                 serviceProvider.GetService<IEnumerable<IAudioServerProcessor>>(),
                 serviceProvider.GetResonyServerLoggerFactory())
        {
        }

        internal AudioServerBase(
            IAudioSourceRuntimeManager audioSourceRuntimeManager,
            IEnumerable<IAudioServerProcessor> audioServerProcessors,
            ILoggerFactory loggerFactory)
        {
            _audioSourceRuntimeManager = audioSourceRuntimeManager;
            _audioServerProcessors = audioServerProcessors;
            _logger = loggerFactory.CreateLogger<AudioServerBase>();
            
            Status = AudioServerStatus.Stopped;
        }

        #endregion

        #region Utils

        private async Task RunServerProcessors(CancellationToken cancellationToken = default)
        {
            var procContext = new AudioServerProcessorContext(cancellationToken);
            foreach (var processor in _audioServerProcessors)
            {
                try
                {
                    await processor.Execute(procContext);
                }
                catch (Exception) { }
            }
        }

        #endregion

        public AudioServerStatus Status { get; private set; }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Status == AudioServerStatus.Started)
                {
                    throw new InvalidOperationException("Audio server already started.");
                }

                _logger.LogDebug("Starting audio server...");
                _stoppingTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _stoppingTokenRegistration = _stoppingTokenSource.Token.Register(async () => await Stop());

                await _audioSourceRuntimeManager.LoadRuntimeInfos();
                Status = AudioServerStatus.Started;
                _logger.LogInformation("Audio server started.");
                await RunServerProcessors(_stoppingTokenSource.Token);                
            }
            catch (Exception) { }
        }

        
        public async Task Stop()
        {
            try
            {
                lock (_stoppingLock)
                {
                    if (_stopping || Status == AudioServerStatus.Stopped)
                    {
                        return;
                    }

                    _logger.LogDebug("Stopping audio server...");
                    _stopping = true;
                    _stoppingTokenSource.Cancel();
                }

                await _audioSourceRuntimeManager.UnloadRuntimeInfos();
                Status = AudioServerStatus.Stopped;
                _logger.LogInformation("Audio server stopped.");
                await Task.CompletedTask;
            }
            catch (Exception) { }
            finally
            {
                _stopping = false;
            }
        }

        public virtual void Dispose()
        {
            try
            {
                Task.Run(async () => await Stop()).Wait();
            }
            catch (Exception)
            {
            }
        }
    }
}
