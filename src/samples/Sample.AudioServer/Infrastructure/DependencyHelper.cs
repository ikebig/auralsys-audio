using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Resony;
using Resony.ManagedBass;
using Resony.Server;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Sample.AudioServer
{
    public static class DependencyHelper
    {
        public static readonly IServiceProvider ServiceProvider = Configure();
        private static IServiceProvider Configure()
        {
            var services = new ServiceCollection();

            services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddSimpleConsole();
                config.SetMinimumLevel(LogLevel.Trace);
            });

            services.AddResony(configure =>
            {
                configure.UseManagedBass();
                configure.UseResonyServer(svc =>
                {
                    svc.RecordingEnabled = true;
                    svc.RecordingLengthMinutes = 5;
                    svc.UseAudioSourceDefinitionFactory(sp =>
                    {
                        return sp.GetService<IDeviceManager>().GetInputDevices()
                        .Select(d => new AudioSourceDefinition
                        {
                            Name = d.ToString(),
                            Source = d.ToString(),
                            Enabled = true
                        }).Take(8).ToList();                       
                    });
                });
            });

            var provider = services.CreateLightInjectServiceProvider();
            return provider;
        }
    }
}
