using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resony.Server
{
    public interface IAudioSourceDefinitionProvider
    {
        Task<IEnumerable<AudioSourceDefinition>> GetAudioSourceDefinitions();
    }
}
