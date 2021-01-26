using System;

namespace GFF.Component.NAudio.CoreAudioApi
{
	[Flags]
	public enum EEndpointHardwareSupport
	{
		Volume = 1,
		Mute = 2,
		Meter = 4
	}
}
