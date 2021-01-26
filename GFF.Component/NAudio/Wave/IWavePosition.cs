using System;

namespace GFF.Component.NAudio.Wave
{
	public interface IWavePosition
	{
		WaveFormat OutputWaveFormat
		{
			get;
		}

		long GetPosition();
	}
}
