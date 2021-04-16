using Resony.Server.Processing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Resony.Server
{
    public static class ServiceCollectionExtensions
    {
        public static ResonyOptions UseResonyServer(this ResonyOptions resonyOptions, 
            Action<ResonyServerOptions>? configure = default)
        {
            var options = new ResonyServerOptions(resonyOptions);
            configure?.Invoke(options);

            resonyOptions.Services
                .AddSingleton(options)
                .AddSingleton<IAudioServer, AudioServer>()
                .AddSingleton<IAudioSourceRuntimeManager, AudioSourceRuntimeManager>();

            //internal processors
            resonyOptions.Services
                .AddTransient<IAudioServerProcessor, ChannelRecordingProcessor>()
                .AddTransient<IAudioServerProcessor, AudioRecorderInvisibitilityProcessor>();                

            return resonyOptions;
        }
    }
}
