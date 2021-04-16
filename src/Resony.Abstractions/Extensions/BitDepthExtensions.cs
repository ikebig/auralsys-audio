namespace Resony
{
    public static class BitDepthExtensions
    {
        public static int GetBlockAlign(this BitDepth bitDepth)
        {
            return (int)bitDepth / 8;
        }
    }
}
