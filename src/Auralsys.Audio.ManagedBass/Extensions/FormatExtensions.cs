using ManagedBass;

namespace Auralsys.Audio.ManagedBass
{
    public static class FormatExtensions
    {
        public static WaveFormat ToWaveFormat(this Format format)
        {
            return new WaveFormat(SampleRate: format.SampleRate, BitsPerSample: (int)format.BitDepth, Channels: format.Channels);
        }
    }
}
