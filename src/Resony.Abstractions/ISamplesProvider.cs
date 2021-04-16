namespace Resony
{
    public interface ISamplesProvider
    {
        int GetNextSamples(byte[] buffer);
    }
}
