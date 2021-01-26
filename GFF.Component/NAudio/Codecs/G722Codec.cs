using System;

namespace GFF.Component.NAudio.Codecs
{
	public class G722Codec
	{
		private static readonly int[] wl = new int[]
		{
			-60,
			-30,
			58,
			172,
			334,
			538,
			1198,
			3042
		};

		private static readonly int[] rl42 = new int[]
		{
			0,
			7,
			6,
			5,
			4,
			3,
			2,
			1,
			7,
			6,
			5,
			4,
			3,
			2,
			1,
			0
		};

		private static readonly int[] ilb = new int[]
		{
			2048,
			2093,
			2139,
			2186,
			2233,
			2282,
			2332,
			2383,
			2435,
			2489,
			2543,
			2599,
			2656,
			2714,
			2774,
			2834,
			2896,
			2960,
			3025,
			3091,
			3158,
			3228,
			3298,
			3371,
			3444,
			3520,
			3597,
			3676,
			3756,
			3838,
			3922,
			4008
		};

		private static readonly int[] wh = new int[]
		{
			0,
			-214,
			798
		};

		private static readonly int[] rh2 = new int[]
		{
			2,
			1,
			2,
			1
		};

		private static readonly int[] qm2 = new int[]
		{
			-7408,
			-1616,
			7408,
			1616
		};

		private static readonly int[] qm4 = new int[]
		{
			0,
			-20456,
			-12896,
			-8968,
			-6288,
			-4240,
			-2584,
			-1200,
			20456,
			12896,
			8968,
			6288,
			4240,
			2584,
			1200,
			0
		};

		private static readonly int[] qm5 = new int[]
		{
			-280,
			-280,
			-23352,
			-17560,
			-14120,
			-11664,
			-9752,
			-8184,
			-6864,
			-5712,
			-4696,
			-3784,
			-2960,
			-2208,
			-1520,
			-880,
			23352,
			17560,
			14120,
			11664,
			9752,
			8184,
			6864,
			5712,
			4696,
			3784,
			2960,
			2208,
			1520,
			880,
			280,
			-280
		};

		private static readonly int[] qm6 = new int[]
		{
			-136,
			-136,
			-136,
			-136,
			-24808,
			-21904,
			-19008,
			-16704,
			-14984,
			-13512,
			-12280,
			-11192,
			-10232,
			-9360,
			-8576,
			-7856,
			-7192,
			-6576,
			-6000,
			-5456,
			-4944,
			-4464,
			-4008,
			-3576,
			-3168,
			-2776,
			-2400,
			-2032,
			-1688,
			-1360,
			-1040,
			-728,
			24808,
			21904,
			19008,
			16704,
			14984,
			13512,
			12280,
			11192,
			10232,
			9360,
			8576,
			7856,
			7192,
			6576,
			6000,
			5456,
			4944,
			4464,
			4008,
			3576,
			3168,
			2776,
			2400,
			2032,
			1688,
			1360,
			1040,
			728,
			432,
			136,
			-432,
			-136
		};

		private static readonly int[] qmf_coeffs = new int[]
		{
			3,
			-11,
			12,
			32,
			-210,
			951,
			3876,
			-805,
			362,
			-156,
			53,
			-11
		};

		private static readonly int[] q6 = new int[]
		{
			0,
			35,
			72,
			110,
			150,
			190,
			233,
			276,
			323,
			370,
			422,
			473,
			530,
			587,
			650,
			714,
			786,
			858,
			940,
			1023,
			1121,
			1219,
			1339,
			1458,
			1612,
			1765,
			1980,
			2195,
			2557,
			2919,
			0,
			0
		};

		private static readonly int[] iln = new int[]
		{
			0,
			63,
			62,
			31,
			30,
			29,
			28,
			27,
			26,
			25,
			24,
			23,
			22,
			21,
			20,
			19,
			18,
			17,
			16,
			15,
			14,
			13,
			12,
			11,
			10,
			9,
			8,
			7,
			6,
			5,
			4,
			0
		};

		private static readonly int[] ilp = new int[]
		{
			0,
			61,
			60,
			59,
			58,
			57,
			56,
			55,
			54,
			53,
			52,
			51,
			50,
			49,
			48,
			47,
			46,
			45,
			44,
			43,
			42,
			41,
			40,
			39,
			38,
			37,
			36,
			35,
			34,
			33,
			32,
			0
		};

		private static readonly int[] ihn;

		private static readonly int[] ihp;

		private static short Saturate(int amp)
		{
			short num = (short)amp;
			if (amp == (int)num)
			{
				return num;
			}
			if (amp > 32767)
			{
				return 32767;
			}
			return -32768;
		}

		private static void Block4(G722CodecState s, int band, int d)
		{
			s.Band[band].d[0] = d;
			s.Band[band].r[0] = (int)G722Codec.Saturate(s.Band[band].s + d);
			s.Band[band].p[0] = (int)G722Codec.Saturate(s.Band[band].sz + d);
			for (int i = 0; i < 3; i++)
			{
				s.Band[band].sg[i] = s.Band[band].p[i] >> 15;
			}
			int num = (int)G722Codec.Saturate(s.Band[band].a[1] << 2);
			int num2 = (s.Band[band].sg[0] == s.Band[band].sg[1]) ? (-num) : num;
			if (num2 > 32767)
			{
				num2 = 32767;
			}
			int num3 = (s.Band[band].sg[0] == s.Band[band].sg[2]) ? 128 : -128;
			num3 += num2 >> 7;
			num3 += s.Band[band].a[2] * 32512 >> 15;
			if (num3 > 12288)
			{
				num3 = 12288;
			}
			else if (num3 < -12288)
			{
				num3 = -12288;
			}
			s.Band[band].ap[2] = num3;
			s.Band[band].sg[0] = s.Band[band].p[0] >> 15;
			s.Band[band].sg[1] = s.Band[band].p[1] >> 15;
			num = ((s.Band[band].sg[0] == s.Band[band].sg[1]) ? 192 : -192);
			num2 = s.Band[band].a[1] * 32640 >> 15;
			s.Band[band].ap[1] = (int)G722Codec.Saturate(num + num2);
			num3 = (int)G722Codec.Saturate(15360 - s.Band[band].ap[2]);
			if (s.Band[band].ap[1] > num3)
			{
				s.Band[band].ap[1] = num3;
			}
			else if (s.Band[band].ap[1] < -num3)
			{
				s.Band[band].ap[1] = -num3;
			}
			num = ((d == 0) ? 0 : 128);
			s.Band[band].sg[0] = d >> 15;
			for (int i = 1; i < 7; i++)
			{
				s.Band[band].sg[i] = s.Band[band].d[i] >> 15;
				num2 = ((s.Band[band].sg[i] == s.Band[band].sg[0]) ? num : (-num));
				num3 = s.Band[band].b[i] * 32640 >> 15;
				s.Band[band].bp[i] = (int)G722Codec.Saturate(num2 + num3);
			}
			for (int i = 6; i > 0; i--)
			{
				s.Band[band].d[i] = s.Band[band].d[i - 1];
				s.Band[band].b[i] = s.Band[band].bp[i];
			}
			for (int i = 2; i > 0; i--)
			{
				s.Band[band].r[i] = s.Band[band].r[i - 1];
				s.Band[band].p[i] = s.Band[band].p[i - 1];
				s.Band[band].a[i] = s.Band[band].ap[i];
			}
			num = (int)G722Codec.Saturate(s.Band[band].r[1] + s.Band[band].r[1]);
			num = s.Band[band].a[1] * num >> 15;
			num2 = (int)G722Codec.Saturate(s.Band[band].r[2] + s.Band[band].r[2]);
			num2 = s.Band[band].a[2] * num2 >> 15;
			s.Band[band].sp = (int)G722Codec.Saturate(num + num2);
			s.Band[band].sz = 0;
			for (int i = 6; i > 0; i--)
			{
				num = (int)G722Codec.Saturate(s.Band[band].d[i] + s.Band[band].d[i]);
				s.Band[band].sz += s.Band[band].b[i] * num >> 15;
			}
			s.Band[band].sz = (int)G722Codec.Saturate(s.Band[band].sz);
			s.Band[band].s = (int)G722Codec.Saturate(s.Band[band].sp + s.Band[band].sz);
		}

		public int Decode(G722CodecState state, short[] outputBuffer, byte[] inputG722Data, int inputLength)
		{
			int result = 0;
			int num = 0;
			int i = 0;
			while (i < inputLength)
			{
				int num2;
				if (state.Packed)
				{
					if (state.InBits < state.BitsPerSample)
					{
						state.InBuffer |= (uint)((uint)inputG722Data[i++] << state.InBits);
						state.InBits += 8;
					}
					num2 = (int)(state.InBuffer & (1u << state.BitsPerSample) - 1u);
					state.InBuffer >>= state.BitsPerSample;
					state.InBits -= state.BitsPerSample;
				}
				else
				{
					num2 = (int)inputG722Data[i++];
				}
				int num3;
				int num5;
				switch (state.BitsPerSample)
				{
				case 6:
				{
					num3 = (num2 & 15);
					int num4 = num2 >> 4 & 3;
					num5 = G722Codec.qm4[num3];
					goto IL_113;
				}
				case 7:
				{
					num3 = (num2 & 31);
					int num4 = num2 >> 5 & 3;
					num5 = G722Codec.qm5[num3];
					num3 >>= 1;
					goto IL_113;
				}
				case 8:
				{
					IL_BB:
					num3 = (num2 & 63);
					int num4 = num2 >> 6 & 3;
					num5 = G722Codec.qm6[num3];
					num3 >>= 2;
					goto IL_113;
				}
				}
				goto IL_BB;
				IL_113:
				num5 = state.Band[0].det * num5 >> 15;
				int num6 = state.Band[0].s + num5;
				if (num6 > 16383)
				{
					num6 = 16383;
				}
				else if (num6 < -16384)
				{
					num6 = -16384;
				}
				num5 = G722Codec.qm4[num3];
				int d = state.Band[0].det * num5 >> 15;
				num5 = G722Codec.rl42[num3];
				num3 = state.Band[0].nb * 127 >> 7;
				num3 += G722Codec.wl[num5];
				if (num3 < 0)
				{
					num3 = 0;
				}
				else if (num3 > 18432)
				{
					num3 = 18432;
				}
				state.Band[0].nb = num3;
				num3 = (state.Band[0].nb >> 6 & 31);
				num5 = 8 - (state.Band[0].nb >> 11);
				int num7 = (num5 < 0) ? (G722Codec.ilb[num3] << -num5) : (G722Codec.ilb[num3] >> num5);
				state.Band[0].det = num7 << 2;
				G722Codec.Block4(state, 0, d);
				if (!state.EncodeFrom8000Hz)
				{
					int num4;
					num5 = G722Codec.qm2[num4];
					int num8 = state.Band[1].det * num5 >> 15;
					num = num8 + state.Band[1].s;
					if (num > 16383)
					{
						num = 16383;
					}
					else if (num < -16384)
					{
						num = -16384;
					}
					num5 = G722Codec.rh2[num4];
					num3 = state.Band[1].nb * 127 >> 7;
					num3 += G722Codec.wh[num5];
					if (num3 < 0)
					{
						num3 = 0;
					}
					else if (num3 > 22528)
					{
						num3 = 22528;
					}
					state.Band[1].nb = num3;
					num3 = (state.Band[1].nb >> 6 & 31);
					num5 = 10 - (state.Band[1].nb >> 11);
					num7 = ((num5 < 0) ? (G722Codec.ilb[num3] << -num5) : (G722Codec.ilb[num3] >> num5));
					state.Band[1].det = num7 << 2;
					G722Codec.Block4(state, 1, num8);
				}
				if (state.ItuTestMode)
				{
					outputBuffer[result++] = (short)(num6 << 1);
					outputBuffer[result++] = (short)(num << 1);
				}
				else if (state.EncodeFrom8000Hz)
				{
					outputBuffer[result++] = (short)(num6 << 1);
				}
				else
				{
					for (int j = 0; j < 22; j++)
					{
						state.QmfSignalHistory[j] = state.QmfSignalHistory[j + 2];
					}
					state.QmfSignalHistory[22] = num6 + num;
					state.QmfSignalHistory[23] = num6 - num;
					int num9 = 0;
					int num10 = 0;
					for (int j = 0; j < 12; j++)
					{
						num10 += state.QmfSignalHistory[2 * j] * G722Codec.qmf_coeffs[j];
						num9 += state.QmfSignalHistory[2 * j + 1] * G722Codec.qmf_coeffs[11 - j];
					}
					outputBuffer[result++] = (short)(num9 >> 11);
					outputBuffer[result++] = (short)(num10 >> 11);
				}
			}
			return result;
		}

		public int Encode(G722CodecState state, byte[] outputBuffer, short[] inputBuffer, int inputBufferCount)
		{
			int result = 0;
			int num = 0;
			int i = 0;
			while (i < inputBufferCount)
			{
				int num2;
				int j;
				if (state.ItuTestMode)
				{
					num = (num2 = inputBuffer[i++] >> 1);
				}
				else if (state.EncodeFrom8000Hz)
				{
					num2 = inputBuffer[i++] >> 1;
				}
				else
				{
					for (j = 0; j < 22; j++)
					{
						state.QmfSignalHistory[j] = state.QmfSignalHistory[j + 2];
					}
					state.QmfSignalHistory[22] = (int)inputBuffer[i++];
					state.QmfSignalHistory[23] = (int)inputBuffer[i++];
					int num3 = 0;
					int num4 = 0;
					for (j = 0; j < 12; j++)
					{
						num4 += state.QmfSignalHistory[2 * j] * G722Codec.qmf_coeffs[j];
						num3 += state.QmfSignalHistory[2 * j + 1] * G722Codec.qmf_coeffs[11 - j];
					}
					num2 = num3 + num4 >> 14;
					num = num3 - num4 >> 14;
				}
				int num5 = (int)G722Codec.Saturate(num2 - state.Band[0].s);
				int num6 = (num5 >= 0) ? num5 : (-(num5 + 1));
				int num7;
				for (j = 1; j < 30; j++)
				{
					num7 = G722Codec.q6[j] * state.Band[0].det >> 12;
					if (num6 < num7)
					{
						break;
					}
				}
				int num8 = (num5 < 0) ? G722Codec.iln[j] : G722Codec.ilp[j];
				int num9 = num8 >> 2;
				int num10 = G722Codec.qm4[num9];
				int d = state.Band[0].det * num10 >> 15;
				int num11 = G722Codec.rl42[num9];
				num6 = state.Band[0].nb * 127 >> 7;
				state.Band[0].nb = num6 + G722Codec.wl[num11];
				if (state.Band[0].nb < 0)
				{
					state.Band[0].nb = 0;
				}
				else if (state.Band[0].nb > 18432)
				{
					state.Band[0].nb = 18432;
				}
				num7 = (state.Band[0].nb >> 6 & 31);
				num10 = 8 - (state.Band[0].nb >> 11);
				int num12 = (num10 < 0) ? (G722Codec.ilb[num7] << -num10) : (G722Codec.ilb[num7] >> num10);
				state.Band[0].det = num12 << 2;
				G722Codec.Block4(state, 0, d);
				int num13;
				if (state.EncodeFrom8000Hz)
				{
					num13 = (192 | num8) >> 8 - state.BitsPerSample;
				}
				else
				{
					int num14 = (int)G722Codec.Saturate(num - state.Band[1].s);
					num6 = ((num14 >= 0) ? num14 : (-(num14 + 1)));
					num7 = 564 * state.Band[1].det >> 12;
					int num15 = (num6 >= num7) ? 2 : 1;
					int num16 = (num14 < 0) ? G722Codec.ihn[num15] : G722Codec.ihp[num15];
					num10 = G722Codec.qm2[num16];
					int d2 = state.Band[1].det * num10 >> 15;
					int num17 = G722Codec.rh2[num16];
					num6 = state.Band[1].nb * 127 >> 7;
					state.Band[1].nb = num6 + G722Codec.wh[num17];
					if (state.Band[1].nb < 0)
					{
						state.Band[1].nb = 0;
					}
					else if (state.Band[1].nb > 22528)
					{
						state.Band[1].nb = 22528;
					}
					num7 = (state.Band[1].nb >> 6 & 31);
					num10 = 10 - (state.Band[1].nb >> 11);
					num12 = ((num10 < 0) ? (G722Codec.ilb[num7] << -num10) : (G722Codec.ilb[num7] >> num10));
					state.Band[1].det = num12 << 2;
					G722Codec.Block4(state, 1, d2);
					num13 = (num16 << 6 | num8) >> 8 - state.BitsPerSample;
				}
				if (state.Packed)
				{
					state.OutBuffer |= (uint)((uint)num13 << state.OutBits);
					state.OutBits += state.BitsPerSample;
					if (state.OutBits >= 8)
					{
						outputBuffer[result++] = (byte)(state.OutBuffer & 255u);
						state.OutBits -= 8;
						state.OutBuffer >>= 8;
					}
				}
				else
				{
					outputBuffer[result++] = (byte)num13;
				}
			}
			return result;
		}

		static G722Codec()
		{
			// 注意: 此类型已标记为 'beforefieldinit'.
			int[] expr_132 = new int[3];
			expr_132[1] = 1;
			G722Codec.ihn = expr_132;
			G722Codec.ihp = new int[]
			{
				0,
				3,
				2
			};
		}
	}
}
