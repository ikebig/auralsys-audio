namespace Auralsys.Audio
{
    public class DataAvailableArgs
    {
        public DataAvailableArgs(byte[] buffer, int length, Device device, Format format)
        {
            Buffer = buffer;
            Length = length;
            Device = device;
            Format = format;
        }

        public byte[] Buffer { get;}
        public int Length { get;}
        public Device Device { get; }
        public Format Format { get; }
    }
}
