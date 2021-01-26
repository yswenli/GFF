using System;

namespace GFF.Component.NAudio.Dmo
{
	[Flags]
	public enum DmoOutputDataBufferFlags
	{
		None = 0,
		SyncPoint = 1,
		Time = 2,
		TimeLength = 4,
		Incomplete = 16777216
	}
}
