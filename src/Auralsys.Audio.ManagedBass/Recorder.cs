using System;
using System.Runtime.InteropServices;

namespace Auralsys.Audio.ManagedBass
{
    internal class Recorder : RecorderBase
    {
        private int _handle;
        private byte[] _buffer;

        private readonly IBassProxy _bassProxy;
        public Recorder(IServiceProvider serviceProvider, Device device)
            : this(serviceProvider, device, Format.Default())
        {
        }

        public Recorder(IServiceProvider serviceProvider, Device device, Format format) 
            : base(device, format)
        {
            _bassProxy = (IBassProxy)serviceProvider.GetService(typeof(IBassProxy));
            _handle = _bassProxy.RecordInit(Device, Format, Procedure);
        }

        public override RecorderState Status => _bassProxy.GetRecorderState(_handle);

        public override event DataAvailableHandler DataAvailable;

        public override bool Start()
        {
            return _bassProxy.ChannelPlay(_handle);
        }

        public override bool Stop()
        {
            return _bassProxy.ChannelStop(_handle);
        }

        public override void Dispose()
        {
            _bassProxy.FreeRecordingDevice(Device.Index);
        }
        
        private bool Procedure(int handle, IntPtr buffer, int length, IntPtr user)
        {
            if (_buffer == null || _buffer.Length < length)
                _buffer = new byte[length];

            Marshal.Copy(buffer, _buffer, 0, length);
            DataAvailable?.Invoke(new DataAvailableArgs(_buffer, length, Device, Format));

            return true;
        }
    }
}
