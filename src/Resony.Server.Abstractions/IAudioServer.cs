using System;
using System.Threading;
using System.Threading.Tasks;

namespace Resony.Server
{
    public interface IAudioServer : IDisposable
    {
        Task Start(CancellationToken cancellationToken = default);
        Task Stop();
        AudioServerStatus Status { get; } 
    }
}
