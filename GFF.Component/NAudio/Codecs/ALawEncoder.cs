using System;

namespace GFF.Component.NAudio.Codecs
{
	public static class ALawEncoder
	{
		private const int cBias = 132;

		private const int cClip = 32635;

		private static readonly byte[] ALawCompressTable = new byte[]
		{
			1,
			1,
			2,
			2,
			3,
			3,
			3,
			3,
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7
		};

		public static byte LinearToALawSample(short sample)
		{
			int num = ~sample >> 8 & 128;
			if (num == 0)
			{
				sample = -sample;
			}
			if (sample > 32635)
			{
				sample = 32635;
			}
			byte b;
			if (sample >= 256)
			{
				int num2 = (int)ALawEncoder.ALawCompressTable[sample >> 8 & 127];
				int num3 = sample >> num2 + 3 & 15;
				b = (byte)(num2 << 4 | num3);
			}
			else
			{
				b = (byte)(sample >> 4);
			}
			return b ^ (byte)(num ^ 85);
		}
	}
}
