using Resony.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Resony.ManagedBass
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
            string tempPath = fileInfo.NewTemporaryPath();

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    Write(stream, samples, format);
                }

                IOHelper.TryMoveFile(tempPath, path, true);
            }
            catch (Exception)
            {
                IOHelper.TryDeleteFile(tempPath);
                throw;
            }            
        }

        public void Write(Stream waveStream, float[] samples, Format format)
        {
            _bassProxy.WriteWaveFile(waveStream, samples, format);
        }

        public void Write(string path, float[] samples, Format format)
        {
            var fileInfo = new FileInfo(path);
            string tempPath = fileInfo.NewTemporaryPath();

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    Write(stream, samples, format);
                }

                IOHelper.TryMoveFile(tempPath, path, true);
            }
            catch (Exception)
            {
                IOHelper.TryDeleteFile(tempPath);
                throw;
            }            
        }

        public void Write(Stream waveStream, short[] samples, Format format)
        {
            _bassProxy.WriteWaveFile(waveStream, samples, format);
        }

        public void Write(string path, short[] samples, Format format)
        {
            var fileInfo = new FileInfo(path);
            string tempPath = fileInfo.NewTemporaryPath();

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    Write(stream, samples, format);
                }

                IOHelper.TryMoveFile(tempPath, path, true);
            }
            catch (Exception)
            {
                IOHelper.TryDeleteFile(tempPath);
                throw;
            }            
        }
    }
}
