using System;

namespace GFF.Component.NAudio.Wave.SampleProviders
{
	public interface IPanStrategy
	{
		StereoSamplePair GetMultipliers(float pan);
	}
}
