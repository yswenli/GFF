using System;

namespace GFF.Component.NAudio.Wave
{
	public enum WaveCallbackStrategy
	{
		FunctionCallback,
		NewWindow,
		ExistingWindow,
		Event
	}
}
