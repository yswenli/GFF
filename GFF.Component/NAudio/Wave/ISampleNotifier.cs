using System;

namespace GFF.Component.NAudio.Wave
{
	public interface ISampleNotifier
	{
		event EventHandler<SampleEventArgs> Sample;
	}
}
