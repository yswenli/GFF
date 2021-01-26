using System;

namespace GFF.Component.NAudio.Wave
{
	public class StoppedEventArgs : EventArgs
	{
		private readonly Exception exception;

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public StoppedEventArgs(Exception exception = null)
		{
			this.exception = exception;
		}
	}
}
