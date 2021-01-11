using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Resony
{
    internal class VirtualDeviceRecorder : RecorderBase
    {
        private byte[] _buffer;

        private readonly IBassProxy _bassProxy;
        public VirtualDeviceRecorder(IServiceProvider serviceProvider, Device device)
            : this(serviceProvider, device, Format.Default())
        {
        }

        public VirtualDeviceRecorder(IServiceProvider serviceProvider, Device device, Format format) 
            : base(device, format)
        {
            _bassProxy = (IBassProxy)serviceProvider.GetService(typeof(IBassProxy));

            InitRecorder();
        }

        private void InitRecorder()
        {
            var virtualDevice = Device as VirtualDevice;
            if (virtualDevice == null)
            {
                Source = _bassProxy.RecordInit(Device, Format, Procedure);
            }
            else
            {
                string[] internetUriSchemes = { "HTTP", "HTTPS", "FTP" };
                var normalizedUriScheme = virtualDevice.Source.Scheme.ToUpper();
                if (normalizedUriScheme.IsIn(internetUriSchemes))
                {
                    Source = _bassProxy.CreateStream(virtualDevice.Source.AbsoluteUri, Format, DownloadProcedure);
                    if (Source == 0)
                    {
                        throw new BassException(_bassProxy.GetLastError());
                    }
                }
                else
                {
                    throw new ArgumentException("Unsupported virtual device source.");
                }
            }
        }
        
        internal int Source { get; private set; }
        public override RecorderState Status => _bassProxy.GetRecorderState(Source);

        public override event DataAvailableHandler DataAvailable;
        private bool HasVirtualDevice => Device as VirtualDevice != null;

        public override bool Start()
        {
            return _bassProxy.ChannelPlay(Source);           
        }

        public override bool Stop()
        {
            return _bassProxy.ChannelStop(Source);
        }

        public override void Dispose()
        {
            if (HasVirtualDevice)
            {
                return;
            }

            _bassProxy.FreeRecordingDevice(Device.Index);
        }
        
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

        private void DownloadProcedure(IntPtr buffer, int length, IntPtr user)
        {
            if (_buffer == null || _buffer.Length < length)
            {
                _buffer = new byte[length];
            }

            if (length > 0)
            {
                Marshal.Copy(buffer, _buffer, 0, length);
                DataAvailable?.Invoke(new DataAvailableArgs(_buffer, length, Device, Format));
            }            
        }

        public override void RecordWaveFile(Stream stream, TimeSpan duration, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
