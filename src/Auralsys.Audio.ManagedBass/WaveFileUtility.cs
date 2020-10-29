using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Auralsys.Audio.ManagedBass
{
    public class WaveFileUtility : IWaveFileUtility
    {
        private readonly IBassProxy _bassProxy;

        public WaveFileUtility(IServiceProvider serviceProvider)
            : this(serviceProvider.GetService<IBassProxy>())
        {
        }
        internal WaveFileUtility(IBassProxy bassProxy)
        {
            _bassProxy = bassProxy;
        }

        public void Write(Stream waveStream, byte[] samples, Format format)
        {
            _bassProxy.WriteWaveFile(waveStream, samples, format);
        }

        public void Write(string path, byte[] samples, Format format)
        {
            var fileInfo = new FileInfo(path);
            string tempPath = CommonHelper.GetTemporaryFilePath(fileInfo);

            using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                Write(stream, samples, format);
            }

            if (File.Exists(tempPath))
            {
                File.Move(tempPath, path, true);
            }
        }

        public void Write(Stream waveStream, float[] samples, Format format)
        {
            _bassProxy.WriteWaveFile(waveStream, samples, format);
        }

        public void Write(string path, float[] samples, Format format)
        {
            var fileInfo = new FileInfo(path);
            string tempPath = CommonHelper.GetTemporaryFilePath(fileInfo);

            using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                Write(stream, samples, format);
            }

            if (File.Exists(tempPath))
            {
                File.Move(tempPath, path, true);
            }
        }

        public void Write(Stream waveStream, short[] samples, Format format)
        {
            _bassProxy.WriteWaveFile(waveStream, samples, format);
        }

        public void Write(string path, short[] samples, Format format)
        {
            var fileInfo = new FileInfo(path);
            string tempPath = CommonHelper.GetTemporaryFilePath(fileInfo);

            using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                Write(stream, samples, format);
            }

            if (File.Exists(tempPath))
            {
                File.Move(tempPath, path, true);
            }
        }
    }
}
