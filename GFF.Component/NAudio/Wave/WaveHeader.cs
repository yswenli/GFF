using System;
using System.Runtime.InteropServices;

namespace GFF.Component.NAudio.Wave
{
	[StructLayout(LayoutKind.Sequential)]
	internal class WaveHeader
	{
		public IntPtr dataBuffer;

		public int bufferLength;

		public int bytesRecorded;

		public IntPtr userData;

		public WaveHeaderFlags flags;

		public int loops;

		public IntPtr next;

		public IntPtr reserved;
	}
}
