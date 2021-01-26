using System;

namespace GFF.Component.NAudio.Mixer
{
	public enum MixerLineComponentType
	{
		DestinationUndefined,
		DestinationDigital,
		DestinationLine,
		DestinationMonitor,
		DestinationSpeakers,
		DestinationHeadphones,
		DestinationTelephone,
		DestinationWaveIn,
		DestinationVoiceIn,
		SourceUndefined = 4096,
		SourceDigital,
		SourceLine,
		SourceMicrophone,
		SourceSynthesizer,
		SourceCompactDisc,
		SourceTelephone,
		SourcePcSpeaker,
		SourceWaveOut,
		SourceAuxiliary,
		SourceAnalog
	}
}
