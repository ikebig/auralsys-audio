using System;

namespace Auralsys.Audio.ManagedBass
{
    public class RecorderFactory : IRecorderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public RecorderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public RecorderBase Create(Device device)
        {
            return new Recorder(_serviceProvider, device);
        }

        public RecorderBase Create(Device device, Format format)
        {
            return new Recorder(_serviceProvider, device, format);
        }
    }
}
