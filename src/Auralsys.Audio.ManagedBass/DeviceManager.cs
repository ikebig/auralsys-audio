using System;
using System.Collections.Generic;
using System.Linq;

namespace Auralsys.Audio.ManagedBass
{
    public class DeviceManager : IDeviceManager
    {
        private readonly IBassProxy _bassProxy;
        public DeviceManager(IServiceProvider serviceProvider)
            :this((IBassProxy)serviceProvider.GetService(typeof(IBassProxy)))
        {            
        }
        internal DeviceManager(IBassProxy bassProxy)
        {
            _bassProxy = bassProxy;
        }
        public IEnumerable<Device> GetInputDevices()
        {
            return _bassProxy.GetInputDevices();
        }
        public IEnumerable<Device> GetOutputDevices()
        {
            return _bassProxy.GetOutputDevices();
        }
        public Device GetInputDevice(int index)
        {
            return _bassProxy.GetInputDevice(index);
        }
        public Device GetInputDevice(string name)
        {
            return GetInputDevice(name);
        }
        public Device GetOutputDevice(int index)
        {
            return _bassProxy.GetOutputDevice(index);
        }
        public Device GetOutputDevice(string name)
        {
            return _bassProxy.GetOutputDevice(name);
        }
    }
}
