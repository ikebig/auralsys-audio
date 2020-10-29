namespace Auralsys.Audio
{
    public enum BitDepth
    {
        Bit_8 = 8,
        Bit_16 = 16,
        Bit_32 = 32
    }

    public static class BitDepthExtensions
    {
        public static int GetBlockAlign(this BitDepth bitDepth)
        {
            return (int)bitDepth / 8;
        }
    }
}
