﻿using System.Collections.Generic;

namespace Resony
{
    public interface IDeviceManager
    {
        IEnumerable<Device> GetInputDevices();
        IEnumerable<Device> GetOutputDevices();
        Device GetInputDevice(int index);
        Device GetInputDevice(string name);
        Device GetOutputDevice(int index);
        Device GetOutputDevice(string name);
    }
}
