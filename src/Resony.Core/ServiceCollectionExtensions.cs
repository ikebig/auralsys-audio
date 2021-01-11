using Microsoft.Extensions.DependencyInjection;

namespace Resony
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddResonyCore(this IServiceCollection services)
        {
            services.AddSingleton<IAudioSamplesConverter, AudioSamplesConverter>();

            return services;
        }
    }
}
