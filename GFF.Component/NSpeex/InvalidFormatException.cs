using System;

namespace GFF.Component.NSpeex
{
	public class InvalidFormatException : Exception
	{
		public InvalidFormatException(string message) : base(message)
		{
		}
	}
}
