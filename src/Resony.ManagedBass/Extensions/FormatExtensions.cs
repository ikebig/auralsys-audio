using ManagedBass;

namespace Resony.ManagedBass
{
    public static class FormatExtensions
    {
        public static WaveFormat ToWaveFormat(this Format format)
        {
            var waveFormat = new WaveFormat(SampleRate: format.SampleRate, BitsPerSample: (int)format.BitDepth, Channels: format.Channels);
            return waveFormat;
        }
    }
}
