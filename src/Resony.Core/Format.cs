namespace Resony
{
    public class Format
    {
        public static Format Default() => 
            new Format(sampleRate: Constants.Audio.Formats.DefaultSampleRate,
                channels: Constants.Audio.Formats.DefaultChannels,
                bitDepth: Constants.Audio.Formats.DefaultBitDepth);
        public Format(int sampleRate)
            :this(sampleRate, Constants.Audio.Formats.DefaultChannels)
        { 
        }

        public Format(int sampleRate, int channels)
            : this(sampleRate, channels, Constants.Audio.Formats.DefaultBitDepth)
        {            
        }
        public Format(int sampleRate, int channels, BitDepth bitDepth)
        {
            SampleRate = sampleRate;
            Channels = channels;
            BitDepth = bitDepth;
        }

        public int Channels { get; }
        public int SampleRate { get; }
        public BitDepth BitDepth { get; }
    }
}
