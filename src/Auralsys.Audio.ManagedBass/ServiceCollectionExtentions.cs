using Auralsys.Audio.ManagedBass;
using Microsoft.Extensions.DependencyInjection;

namespace Auralsys.Audio
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAuralsysAudioManagedBass(this IServiceCollection services)
        {
            services.AddSingleton<IBassProxy, BassProxy>();
            services.AddSingleton<IDeviceManager, DeviceManager>();
            services.AddSingleton<IRecorderFactory, RecorderFactory>();
            services.AddSingleton<IWaveFileUtility, WaveFileUtility>();

            return services;
        }
    }
}
