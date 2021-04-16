using Microsoft.Extensions.DependencyInjection;
using System;

namespace Resony
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddResony(
           this IServiceCollection services,
           Action<ResonyOptions>? configure = default) =>
           services
               .AddResonyCore(configure);

        private static IServiceCollection AddResonyCore(this IServiceCollection services,
            Action<ResonyOptions>? configure = default)
        {
            var options = new ResonyOptions(services);
            configure?.Invoke(options);

            services
                .AddSingleton(options)
                .AddSingleton<IAudioSamplesConverter, AudioSamplesConverter>();

            return services;
        }

        
    }
}
