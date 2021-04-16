namespace Resony
{
    public interface IAudioSamplesConverter
    {
        float[] ByteToFloat(byte[] input, BitDepth inputBitDepth);
        byte[] MonoToStereo(byte[] input);
        byte[] StereoToMono(byte[] input);
        byte[] MixStereoToMono(byte[] input);
    }
}
