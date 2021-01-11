using System;
using System.Threading;

namespace Resony
{
    public sealed class VirtualDevice : Device
    {
        private static int _deviceCounter = int.MinValue;

        public VirtualDevice(Uri source) : this(NewIndex, source)
        {
        }

        public VirtualDevice(int index, Uri source) : base(index, source.AbsoluteUri)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        private static int NewIndex => Interlocked.Increment(ref _deviceCounter) - 1;

        public Uri Source { get; }
    }
}
