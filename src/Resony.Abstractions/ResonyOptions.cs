using Microsoft.Extensions.DependencyInjection;
using System;

namespace Resony
{
    public class ResonyOptions
    {
        public ResonyOptions(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public ResonyOptions UseWaveFileUtility(Func<IServiceProvider, IWaveFileUtility> factory)
        {
            Services.AddSingleton<IWaveFileUtility>(factory);
            return this;
        }

        public ResonyOptions UseDeviceManager(Func<IServiceProvider, IDeviceManager> factory)
        {
            Services.AddSingleton<IDeviceManager>(factory);
            return this;
        }

        public ResonyOptions UseRecorderFactory(Func<IServiceProvider, IRecorderFactory> factory)
        {
            Services.AddSingleton<IRecorderFactory>(factory);
            return this;
        }
        
    }
}
