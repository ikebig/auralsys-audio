using Auralsys.Audio;

namespace Auralsys
{
    public static partial class Constants
    {
        public static class Audio
        {
            public static class Recording
            {
                public const int SampleAggregationTimeoutMilliseconds = 100;
            }
            public static class Formats
            {
                public const int DefaultChannels = 1;
                public const int DefaultSampleRate = 44100;
                public const BitDepth DefaultBitDepth = BitDepth.Bit_16;
            }
        }    
    }
}
