using System;

namespace GFF.Component.NAudio.CoreAudioApi
{
	public struct PropertyKey
	{
		public Guid formatId;

		public int propertyId;

		public PropertyKey(Guid formatId, int propertyId)
		{
			this.formatId = formatId;
			this.propertyId = propertyId;
		}
	}
}
