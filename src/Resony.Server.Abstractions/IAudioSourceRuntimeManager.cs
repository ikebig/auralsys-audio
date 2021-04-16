using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Resony.Server
{
    internal interface IAudioSourceRuntimeManager
    {
        IReadOnlyCollection<AudioSourceRuntimeInfo> AudioSourceRuntimeInfos { get; }
        Task<int> LoadRuntimeInfos(CancellationToken cancellationToken = default);
        Task<bool> ReloadRecorder(string audioSourceDefinitionName, CancellationToken cancellationToken = default);
        Task UnloadRuntimeInfos();
    }
}