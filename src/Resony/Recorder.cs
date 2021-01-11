using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WaveFileWriter = ManagedBass.WaveFileWriter;

namespace Resony
{
    internal class Recorder : RecorderBase
    {
        private byte[] _buffer;

        #region Ctor

        private readonly IBassProxy _bassProxy;
        public Recorder(IServiceProvider serviceProvider, Device device)
            : this(serviceProvider, device, Format.Default())
        {
        }

        public Recorder(IServiceProvider serviceProvider, Device device, Format format) 
            : base(device, format)
        {
            _bassProxy = (IBassProxy)serviceProvider.GetService(typeof(IBassProxy));
            Stream = _bassProxy.RecordInit(Device, Format, Procedure);
        }

        #endregion

        internal int Stream { get; }        
        
        private bool Procedure(int handle, IntPtr buffer, int length, IntPtr user)
        {
            if (_buffer == null || _buffer.Length < length)
            {
                _buffer = new byte[length];
            }

            Marshal.Copy(buffer, _buffer, 0, length);
            DataAvailable?.Invoke(new DataAvailableArgs(_buffer, length, Device, Format));

            return true;
        }

        #region RecorderBase Implementations

        public override RecorderState Status => _bassProxy.GetRecorderState(Stream);

        public override event DataAvailableHandler DataAvailable;

        public override bool Start()
        {
            return _bassProxy.ChannelPlay(Stream);
        }

        public override bool Stop()
        {
            return _bassProxy.ChannelStop(Stream);
        }

        public override void Dispose()
        {
            _bassProxy.FreeRecordingDevice(Device.Index);
        }

        public override void RecordWaveFile(Stream stream, TimeSpan duration, CancellationToken cancellationToken = default)
        {
            if (this == null || duration == TimeSpan.Zero)
            {
                return;
            }

            if (Status != RecorderState.Playing)
            {
                throw new BassException($"Invalid {nameof(RecorderState)}.");
            }

            long totalBytesRead = 0;
            long totalBytesToRead = (long)(duration.TotalSeconds * Format.Channels * Format.SampleRate * Format.BitDepth.GetBlockAlign());
            var writer = new WaveFileWriter(stream, Format.ToWaveFormat());
            bool forceExit = false;

            void aggregation(DataAvailableArgs args)
            {
                try
                {
                    if (args.Length <= 0)
                    {
                        throw new BassException("Number of bytes read is negative or zero.");
                    }

                    if (totalBytesRead < totalBytesToRead)
                    {
                        byte[] chunk = new byte[args.Length];
                        if (totalBytesRead + args.Length > totalBytesToRead)
                        {
                            chunk = new byte[totalBytesToRead - totalBytesRead];
                        }

                        Array.Copy(args.Buffer, 0, chunk, 0, chunk.Length);
                        writer.Write(chunk, chunk.Length);
                        totalBytesRead += chunk.Length;
                    }
                }
                catch (Exception ex)
                {
                    forceExit = true;
                    Trace.TraceError(ex.Message);
                }
            }

            DataAvailable += aggregation;
            while (Status == RecorderState.Playing && totalBytesRead < totalBytesToRead && !forceExit && !cancellationToken.IsCancellationRequested)
            {
                Task.Delay(Constants.Audio.Recording.SampleAggregationTimeoutMilliseconds, cancellationToken).Wait();
            }
            DataAvailable -= aggregation;
            writer.Dispose();
        }

        #endregion

    }
}
