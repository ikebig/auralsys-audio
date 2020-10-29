using System;

namespace Auralsys.Audio.ManagedBass
{
    public class BassException : Exception
    {
        public BassException(string message)
            : base(message)
        {
        }
    }
}
