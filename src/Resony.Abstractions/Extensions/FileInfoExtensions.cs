using System;
using System.IO;

namespace Resony
{
    public static class FileInfoExtensions
    {

        /// <summary>
        /// Computes a new temporary file path relative to the FullName of the provided <paramref name="fileInfo"/>
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns>A string relative the full path.</returns>
        public static string NewTemporaryPath(this FileInfo fileInfo)
        {
            if (fileInfo == null || fileInfo.FullName.IsEmpty())
            {
                return null;
            }

            var path = fileInfo.FullName;
            string tempPath = path.Substring(0, path.Length - fileInfo.Extension.Length) + "." + Guid.NewGuid().ToString("N").Substring(0, 8) + ".tmp";
            return tempPath;
        }
    }
}
