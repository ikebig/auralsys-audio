using System;

namespace Resony.Server
{
    public class AudioSourceRuntimeInfo
    {
        public AudioSourceRuntimeInfo(AudioSourceDefinition audioSourceDefinition, RecorderBase audioRecorder)
        {
            AudioSourceDefinition = audioSourceDefinition;
            Recorder = audioRecorder;
            HeartBeat(true);
        }
        public AudioSourceDefinition AudioSourceDefinition { get; private set; }
        public RecorderBase Recorder { get; private set; }
        public DateTime StartUtc { get; private set; }
        public DateTime LastHeartBeatUtc { get; private set; }

        internal void UpdateRecorder(RecorderBase recorder) => Recorder = recorder;
        internal void HeartBeat(bool resetStartUtc = false)
        {
            var now = DateTime.UtcNow;
            if (resetStartUtc)
            {
                StartUtc = now;
            }
            LastHeartBeatUtc = now;
        }
    }
}
