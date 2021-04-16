using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resony.Server
{
    public class ResonyServerOptions
    {
        #region Const

        private const int RECORDER_INVISIBILITY_TIMEOUT_SECONDS_DEFAULT = 60;
        private const double RECORDING_LENGTH_MINUTES_DEFAULT = 1D;
        private const int RECORDING_SAMPLE_RATE_DEFAULT = 5512;
        private const int RECORDING_CHANNELS_DEFAULT = 1;
        private const BitDepth RECORDING_BIT_DEPTH_DEFAULT = BitDepth.Bit_16;
        private const string RECORDING_ROOT_FOLDER_DEFAULT = "Recordings";

        #endregion

        public ResonyServerOptions(ResonyOptions resonyOptions)
        {
            Services = resonyOptions.Services;
        }

        public IServiceCollection Services { get; }        
        internal Func<IServiceProvider, IEnumerable<AudioSourceDefinition>> AudioSourceDefinitionFactory { get; set; } = sp => Enumerable.Empty<AudioSourceDefinition>();
        
        internal Func<IServiceProvider, ILoggerFactory> LoggerFactoryFactory { get; set; } = sp => sp.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
        public int RecorderInvisibiltyTimeoutSeconds { get; set; } = RECORDER_INVISIBILITY_TIMEOUT_SECONDS_DEFAULT;
        public bool RecordingEnabled { get; set; }
        public double RecordingLengthMinutes { get; set; } = RECORDING_LENGTH_MINUTES_DEFAULT;
        public int RecordingSampleRate { get; set; } = RECORDING_SAMPLE_RATE_DEFAULT;
        public int RecordingChannels { get; set; } = RECORDING_CHANNELS_DEFAULT;
        public BitDepth RecordingBitDepth { get; set; } = RECORDING_BIT_DEPTH_DEFAULT;
        public string RecordingRootFolder { get; set; } = RECORDING_ROOT_FOLDER_DEFAULT;


        #region LoggerFactory

        public ResonyServerOptions UseLoggerFactory(ILoggerFactory? loggerFactory)
        {
            if (loggerFactory != null)
            {
                LoggerFactoryFactory = sp => loggerFactory;
            }

            return this;
        }

        internal ILoggerFactory GetLoggerFactory(IServiceProvider serviceProvider) => LoggerFactoryFactory.Invoke(serviceProvider);

        #endregion

        #region UseAudioSourceDefinitionFactory Methods

        public ResonyServerOptions UseAudioSourceDefinitionFactory(Func<IServiceProvider, IEnumerable<AudioSourceDefinition>> factory)
        {
            AudioSourceDefinitionFactory = factory;
            return this;
        }

        public ResonyServerOptions UseAudioSourceDefinitionFactory(Func<IServiceProvider, IAudioSourceDefinitionProvider> factory)
        {
            AudioSourceDefinitionFactory = sp => Task.Run(async () => await factory.Invoke(sp).GetAudioSourceDefinitions()).Result;
            return this;
        }

        public ResonyServerOptions UseAudioSourceDefinitionFactory<T>() where T : class, IAudioSourceDefinitionProvider 
        {
            AudioSourceDefinitionFactory = sp => Task.Run(async () => (await ActivatorUtilities.CreateInstance<T>(sp)?.GetAudioSourceDefinitions()) ?? Enumerable.Empty<AudioSourceDefinition>()).Result;
            return this;
        }

        #endregion
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           