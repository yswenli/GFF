using System;

namespace GFF.Component.NAudio.Wave
{
	public class CueWaveFileReader : WaveFileReader
	{
		private CueList cues;

		public CueList Cues
		{
			get
			{
				if (this.cues == null)
				{
					this.cues = CueList.FromChunks(this);
				}
				return this.cues;
			}
		}

		public CueWaveFileReader(string fileName) : base(fileName)
		{
		}
	}
}
