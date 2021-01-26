using System;

namespace GFF.Component.NAudio.Wave.SampleProviders
{
	public class StreamVolumeEventArgs : EventArgs
	{
		public float[] MaxSampleValues
		{
			get;
			set;
		}
	}
}
