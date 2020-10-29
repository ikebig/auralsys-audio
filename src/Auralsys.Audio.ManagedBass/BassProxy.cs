using ManagedBass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Auralsys.Audio.ManagedBass
{
    internal class BassProxy : IBassProxy
    {
        const string DllName = "bass";

        private static IntPtr _bassDll;
        private static readonly string _lock = $"{nameof(_lock)}";

        public BassProxy()
        {
            LoadBass();
        }

        #region Devices
        public IEnumerable<Device> GetInputDevices()
        {
            for (int i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                if (!info.IsLoopback)
                    yield return new Device(i, info.Name);
            }
                
        }

        public Device GetInputDevice(int index)
        {
            if (!Bass.RecordGetDeviceInfo(index, out var info) || info.IsLoopback)
            {
                return null;
            }

            return new Device(index, info.Name);
        }

        public Device GetInputDevice(string name)
        {
            Device device = null;
            for (int i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                if (!info.IsLoopback && info.Name == name)
                {
                    device = new Device(i, info.Name);
                    break;
                }
            }

            return device;
        }

        public IEnumerable<Device> GetOutputDevices()
        {
            for (int i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
                yield return new Device(i, info.Name);
        }

        public Device GetOutputDevice(int index)
        {
            if(!Bass.GetDeviceInfo(index, out var info))
            {
                return null;
            }            
            
            return new Device(index, info.Name);
        }

        public Device GetOutputDevice(string name)
        {
            Device device = null;
            for (int i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
            {
                if (info.Name == name)
                {
                    device = new Device(i, info.Name);
                    break;
                }
            }

            return device;
        }

        #endregion

        #region Recording
        public bool FreeRecordingDevice(int device)
        {
            Bass.CurrentRecordingDevice = device;
            return Bass.RecordFree();
        }

        public int RecordInit(Device device, Format format, RecordProcedure procedure)
        {            
            var bassFlags = BassFlags.RecordPause;
            switch (format.BitDepth)
            {
                case BitDepth.Bit_8:
                    bassFlags |= BassFlags.Byte;
                    break;
                case BitDepth.Bit_32:
                    bassFlags |= BassFlags.Float | BassFlags.Decode;
                    break;
                default: //default is 16-bit
                    break;
            }

            Bass.RecordInit(device.Index);
            return Bass.RecordStart(format.SampleRate, format.Channels, bassFlags, Constants.Audio.Recording.SampleAggregationTimeoutMilliseconds, procedure);
        }

        #endregion

        #region WaveWriter

        public void WriteWaveFile(Stream stream, byte[] samples, Format format)
        {
            WaveFormat waveFormat = format.ToWaveFormat();
            using var writer = new WaveFileWriter(stream, waveFormat);
            writer.Write(samples, samples.Length);
        }

        public void WriteWaveFile(Stream stream, float[] samples, Format format)
        {
            WaveFormat waveFormat = format.ToWaveFormat();
            using var writer = new WaveFileWriter(stream, waveFormat);
            writer.Write(samples, samples.Length);
        }

        public void WriteWaveFile(Stream stream, short[] samples, Format format)
        {
            WaveFormat waveFormat = format.ToWaveFormat();
            using var writer = new WaveFileWriter(stream, waveFormat);
            writer.Write(samples, samples.Length);
        }

        #endregion

        public bool ChannelPlay(int handle)
        {
            return Bass.ChannelPlay(handle);
        }

        public bool ChannelStop(int handle)
        {
            return Bass.ChannelStop(handle);
        }

        public RecorderState GetRecorderState(int handle)
        {
            var playbackState = Bass.ChannelIsActive(handle);
            if (playbackState != PlaybackState.Playing)
            {
                return RecorderState.Stoped;
            }

            return RecorderState.Playing;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            UnloadBass();           
        }

        #region Utils
        private bool LoadBass()
        {
            try
            {
                lock (_lock)
                {
                    if (_bassDll == IntPtr.Zero)
                    {
                        string bassNativeDirectory = BassHelper.GetBassRuntimeDirectory();
                        if (!Directory.Exists(bassNativeDirectory) || !Directory.GetFiles(bassNativeDirectory, $"*{DllName}*", SearchOption.TopDirectoryOnly).Any())
                        {
                            return false;
                        }

                        string bassLibraryPath = Path.Combine(BassHelper.GetBassRuntimeDirectory(), DllName);
                        _bassDll = NativeLibrary.Load(bassLibraryPath);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Could load Bass library. {ex.Message}", "Error");
                return false;
            }
        }

        private bool UnloadBass()
        {
            try
            {
                lock (_lock)
                {
                    if (_bassDll != IntPtr.Zero)
                    {
                        NativeLibrary.Free(_bassDll);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Trace.WriteLine($"Could not free Bass library. Possible memory leak! {ex.Message}", "Error");
                return false;
            }            
        }

        #endregion
    }
}
