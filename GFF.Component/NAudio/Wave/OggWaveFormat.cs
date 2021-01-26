using System;
using System.Runtime.InteropServices;

namespace GFF.Component.NAudio.Wave
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class OggWaveFormat : WaveFormat
	{
		public uint dwVorbisACMVersion;

		public uint dwLibVorbisVersion;
	}
}
