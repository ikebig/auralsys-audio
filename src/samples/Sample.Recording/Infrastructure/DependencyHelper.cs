using Auralsys.Audio;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Sample.Recording
{
    public static class DependencyHelper
    {
        public static readonly IServiceProvider ServiceProvider = Configure();
        private static IServiceProvider Configure()
        {
            var services = new ServiceCollection();
            services.AddAuralsysAudioManagedBass();

            var provider = services.CreateLightInjectServiceProvider();
            return provider;
        }
    }
}
