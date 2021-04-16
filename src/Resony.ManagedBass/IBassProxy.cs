using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;

namespace Resony.ManagedBass
{
    internal interface IBassProxy : IDisposable
    {
        IEnumerable<Device> GetInputDevices();
        IEnumerable<Device> GetOutputDevices();
        Device GetInputDevice(int index);
        Device GetInputDevice(string name);
        Device GetOutputDevice(int index);
        Device GetOutputDevice(string name);
        int RecordInit(Device device, Format format, RecordProcedure procedure);
        int CreateStream(string url, Format format, DownloadProcedure procedure);
        int CreateStream(string file, Format format);
        bool FreeRecordingDevice(int device);
        void WriteWaveFile(Stream stream, byte[] samples, Format format);
        void WriteWaveFile(Stream stream, float[] samples, Format format);
        void WriteWaveFile(Stream stream, short[] samples, Format format);
        RecorderState GetRecorderState(int handle);
        bool ChannelPlay(int handle);
        bool ChannelStop(int handle);
        int CreateMixerStream(Format format);
        bool CombineMixerStreams(int mixerStream, int stream, BassFlags flags);
        bool RemoveMixerStreams(int stream);
        int ChannelGetData(int stream, byte[] buffer, int length);
        string GetLastError();
    }
}
