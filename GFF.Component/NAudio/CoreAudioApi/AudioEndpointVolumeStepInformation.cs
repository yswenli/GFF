using GFF.Component.NAudio.CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;

namespace GFF.Component.NAudio.CoreAudioApi
{
    public class AudioEndpointVolumeStepInformation
    {
        private readonly uint step;

        private readonly uint stepCount;

        public uint Step
        {
            get
            {
                return this.step;
            }
        }

        public uint StepCount
        {
            get
            {
                return this.stepCount;
            }
        }

        internal AudioEndpointVolumeStepInformation(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeStepInfo(out this.step, out this.stepCount));
        }
    }
}
