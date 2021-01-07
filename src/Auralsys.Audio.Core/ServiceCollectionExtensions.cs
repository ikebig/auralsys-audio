using Microsoft.Extensions.DependencyInjection;

namespace Auralsys.Audio
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAuralsysAudioCore(this IServiceCollection services)
        {
            services.AddSingleton<IAudioSamplesConverter, AudioSamplesConverter>();

            return services;
        }
    }
}
