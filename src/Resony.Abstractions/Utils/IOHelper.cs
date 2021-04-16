using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Resony.Utils
{
    public static class IOHelper
    {
        /// <summary>
        /// Gets OS specific bass runtime directory for the current application e.g. win-x64, linux-x64, osx-x64, etc
        /// </summary>
        /// <returns></returns>
        public static string GetOSPlatformRuntimeDirectory()
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

            return Path.Combine("runtimes", osPlatformFolder);
        }

        public static bool TryCreateDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return true;
                }

                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryDeleteFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return false;
                }

                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryMoveFile(string sourceFileName, string destFileName, bool overwrite)
        {
            try
            {
                if (!File.Exists(sourceFileName))
                {
                    return false;
                }

                File.Move(sourceFileName, destFileName, overwrite);
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
    }
}
