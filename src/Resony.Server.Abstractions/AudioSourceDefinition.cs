using System.Collections.Generic;

namespace Resony.Server
{
    public class AudioSourceDefinition
    {
        public string Name { get; set; }
        public string Source { get; set; }        
        public bool Enabled { get; set; } = true;
        public Dictionary<object, object> Properties { get; set; } = new Dictionary<object, object>();
    }
}
