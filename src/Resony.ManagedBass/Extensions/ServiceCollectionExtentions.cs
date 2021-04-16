using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Resony.ManagedBass
{
    public static class ServiceCollectionExtentions
    {
        public static ResonyOptions UseManagedBassDeviceManager(this ResonyOptions resonyOptions)
        {
            resonyOptions.Services.TryAddManagedBassCore();

            resonyOptions.Services
                .AddSingleton<IDeviceManager, DeviceManager>();

            return resonyOptions;
        }

        public static ResonyOptions UseManagedBassRecorderFactory(this ResonyOptions resonyOptions)
        {
            resonyOptions.Services.TryAddManagedBassCore();

            resonyOptions.Services
                .AddSingleton<IRecorderFactory, RecorderFactory>();

            return resonyOptions;
        }

        public static ResonyOptions UseManagedBassWaveFileUtility(this ResonyOptions resonyOptions)
        {
            resonyOptions.Services.TryAddManagedBassCore();

            resonyOptions.Services
                .AddSingleton<IWaveFileUtility, WaveFileUtility>();

            return resonyOptions;
        }

        /// <summary>
        /// Use Managed Bass audio services
        /// </summary>
        /// <param name="resonyOptions"></param>
        /// <returns></returns>
        public static ResonyOptions UseManagedBass(this ResonyOptions resonyOptions)
        {
            resonyOptions
                .UseManagedBassDeviceManager()
                .UseManagedBassRecorderFactory()
                .UseManagedBassRecorderFactory();

            return resonyOptions;
        }

        private static void TryAddManagedBassCore(this IServiceCollection services)
        {
            services.TryAddSingleton<IBassProxy, BassProxy>();
        }
    }
}
