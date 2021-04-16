using Microsoft.Extensions.Logging;
using System;

namespace Resony.Server
{
    public static class IServiceProviderExtensions
    {
        internal static ILoggerFactory GetResonyServerLoggerFactory(this IServiceProvider serviceProvider)
        {
            var serverOptions = (ResonyServerOptions)serviceProvider.GetService(typeof(ResonyServerOptions));
            return serverOptions.GetLoggerFactory(serviceProvider);
        }
    }
}
