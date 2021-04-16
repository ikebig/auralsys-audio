using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Resony;
using Resony.ManagedBass;
using System;

namespace Sample.Recording
{
    public static class DependencyHelper
    {
        public static readonly IServiceProvider ServiceProvider = Configure();
        private static IServiceProvider Configure()
        {
            var services = new ServiceCollection();

            services.AddResony(configure =>
            {
                configure.UseManagedBassDeviceManager();
                configure.UseManagedBassRecorderFactory();
                configure.UseManagedBassWaveFileUtility();
            });

            var provider = services.CreateLightInjectServiceProvider();
            return provider;
        }
    }
}
