using System.IO;

namespace Auralsys.Audio
{
    public interface IWaveFileUtility
    {
        void Write(Stream outStream, byte[] samples, Format format);
        void Write(string path, byte[] samples, Format format);
        void Write(Stream outStream, float[] samples, Format format);
        void Write(string path, float[] samples, Format format);
        void Write(Stream outStream, short[] samples, Format format);
        void Write(string path, short[] samples, Format format);
    }
}
