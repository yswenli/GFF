using System;

namespace GFF.Component.NAudio.CoreAudioApi
{
	public enum AudioStreamCategory
	{
		Other,
		ForegroundOnlyMedia,
		BackgroundCapableMedia,
		Communications,
		Alerts,
		SoundEffects,
		GameEffects,
		GameMedia
	}
}
