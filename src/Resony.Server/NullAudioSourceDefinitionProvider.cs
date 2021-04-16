using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resony.Server
{
    public class NullAudioSourceDefinitionProvider : IAudioSourceDefinitionProvider
    {
        public async Task<IEnumerable<AudioSourceDefinition>> GetAudioSourceDefinitions()
        {
            return await Task.FromResult(Enumerable.Empty<AudioSourceDefinition>());
        }
    }
}
