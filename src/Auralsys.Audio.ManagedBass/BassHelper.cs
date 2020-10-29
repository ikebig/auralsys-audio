using System.IO;
using System.Runtime.InteropServices;

namespace Auralsys.Audio.ManagedBass
{
    public static class BassHelper
    {
        /// <summary>
        /// Gets OS specific bass runtime directory for the current application e.g. win-x64, linux-x64, os-x64, etc
        /// </summary>
        /// <returns></returns>
        public static string GetBassRuntimeDirectory()
        {
            string osPlatformFolder = string.Empty;
            string processorArchitecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                osPlatformFolder = $"linux-{processorArchitecture}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                osPlatformFolder = $"osx-{processorArchitecture}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osPlatformFolder = $"win-{processorArchitecture}";
            }

            return Path.Combine("runtimes", osPlatformFolder, "Bass");
        }
    }
}
