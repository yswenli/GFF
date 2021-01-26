using System;

namespace GFF.Component.NAudio.Wave
{
	public interface IWaveProvider
	{
		WaveFormat WaveFormat
		{
			get;
		}

		int Read(byte[] buffer, int offset, int count);
	}
}
