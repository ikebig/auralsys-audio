using System;

namespace Resony.Server
{
    public class AudioServer : AudioServerBase
    {
        public AudioServer(IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
