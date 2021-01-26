using System;
using System.Runtime.InteropServices;

namespace GFF.Component.NAudio.Wave.Asio
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct AsioTimeCode
	{
		public double speed;

		public Asio64Bit timeCodeSamples;

		public AsioTimeCodeFlags flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string future;
	}
}
