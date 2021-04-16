using System;

namespace Resony.ManagedBass
{
    public class BassException : Exception
    {
        public BassException(string message)
            : base(message)
        {
        }
    }
}
