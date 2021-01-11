using Resony;
using Microsoft.Extensions.DependencyInjection;

namespace Auralsys.Audio
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddResony(this IServiceCollection services)
        {
            services.AddResonyCore();

            services.AddSingleton<IBassProxy, BassProxy>();
            services.AddSingleton<IDeviceManager, DeviceManager>();
            services.AddSingleton<IRecorderFactory, RecorderFactory>();
            services.AddSingleton<IWaveFileUtility, WaveFileUtility>();

            return services;
        }
    }
}
