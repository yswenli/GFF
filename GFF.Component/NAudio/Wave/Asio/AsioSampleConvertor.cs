using System;

namespace GFF.Component.NAudio.Wave.Asio
{
	internal class AsioSampleConvertor
	{
		public delegate void SampleConvertor(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples);

		public static AsioSampleConvertor.SampleConvertor SelectSampleConvertor(WaveFormat waveFormat, AsioSampleType asioType)
		{
			AsioSampleConvertor.SampleConvertor result = null;
			bool flag = waveFormat.Channels == 2;
			switch (asioType)
			{
			case AsioSampleType.Int16LSB:
			{
				int bitsPerSample = waveFormat.BitsPerSample;
				if (bitsPerSample != 16)
				{
					if (bitsPerSample == 32)
					{
						result = (flag ? new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorFloatToShort2Channels) : new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorFloatToShortGeneric));
					}
				}
				else
				{
					result = (flag ? new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorShortToShort2Channels) : new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorShortToShortGeneric));
				}
				break;
			}
			case AsioSampleType.Int24LSB:
			{
				int bitsPerSample = waveFormat.BitsPerSample;
				if (bitsPerSample == 16)
				{
					throw new ArgumentException("Not a supported conversion");
				}
				if (bitsPerSample == 32)
				{
					result = new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConverterFloatTo24LSBGeneric);
				}
				break;
			}
			case AsioSampleType.Int32LSB:
			{
				int bitsPerSample = waveFormat.BitsPerSample;
				if (bitsPerSample != 16)
				{
					if (bitsPerSample == 32)
					{
						result = (flag ? new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorFloatToInt2Channels) : new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorFloatToIntGeneric));
					}
				}
				else
				{
					result = (flag ? new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorShortToInt2Channels) : new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConvertorShortToIntGeneric));
				}
				break;
			}
			case AsioSampleType.Float32LSB:
			{
				int bitsPerSample = waveFormat.BitsPerSample;
				if (bitsPerSample == 16)
				{
					throw new ArgumentException("Not a supported conversion");
				}
				if (bitsPerSample == 32)
				{
					result = new AsioSampleConvertor.SampleConvertor(AsioSampleConvertor.ConverterFloatToFloatGeneric);
				}
				break;
			}
			default:
				throw new ArgumentException(string.Format("ASIO Buffer Type {0} is not yet supported.", Enum.GetName(typeof(AsioSampleType), asioType)));
			}
			return result;
		}

		public unsafe static void ConvertorShortToInt2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)((void*)inputInterleavedBuffer);
			short* ptr2 = (short*)((void*)asioOutputBuffers[0]);
			short* ptr3 = (short*)((void*)asioOutputBuffers[1]);
			ptr2++;
			ptr3++;
			for (int i = 0; i < nbSamples; i++)
			{
				*ptr2 = *ptr;
				*ptr3 = ptr[1];
				ptr += 2;
				ptr2 += 2;
				ptr3 += 2;
			}
		}

		public unsafe static void ConvertorShortToIntGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)((void*)inputInterleavedBuffer);
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)((void*)asioOutputBuffers[i]);
				array[i] += 2;
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					*array[k] = *(ptr++);
					array[k] += (IntPtr)2 * 2;
				}
			}
		}

		public unsafe static void ConvertorFloatToInt2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)((void*)inputInterleavedBuffer);
			int* ptr2 = (int*)((void*)asioOutputBuffers[0]);
			int* ptr3 = (int*)((void*)asioOutputBuffers[1]);
			for (int i = 0; i < nbSamples; i++)
			{
				*(ptr2++) = AsioSampleConvertor.clampToInt((double)(*ptr));
				*(ptr3++) = AsioSampleConvertor.clampToInt((double)ptr[1]);
				ptr += 2;
			}
		}

		public unsafe static void ConvertorFloatToIntGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)((void*)inputInterleavedBuffer);
			int*[] array = new int*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (int*)((void*)asioOutputBuffers[i]);
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int*[] expr_36_cp_0 = array;
					int expr_36_cp_1 = k;
					int* ptr2 = expr_36_cp_0[expr_36_cp_1];
					expr_36_cp_0[expr_36_cp_1] = ptr2 + 1;
					*ptr2 = AsioSampleConvertor.clampToInt((double)(*(ptr++)));
				}
			}
		}

		public unsafe static void ConvertorShortToShort2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)((void*)inputInterleavedBuffer);
			short* ptr2 = (short*)((void*)asioOutputBuffers[0]);
			short* ptr3 = (short*)((void*)asioOutputBuffers[1]);
			for (int i = 0; i < nbSamples; i++)
			{
				*(ptr2++) = *ptr;
				*(ptr3++) = ptr[1];
				ptr += 2;
			}
		}

		public unsafe static void ConvertorShortToShortGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)((void*)inputInterleavedBuffer);
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)((void*)asioOutputBuffers[i]);
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					short*[] expr_36_cp_0 = array;
					int expr_36_cp_1 = k;
					short* ptr2 = expr_36_cp_0[expr_36_cp_1];
					expr_36_cp_0[expr_36_cp_1] = ptr2 + 1;
					*ptr2 = *(ptr++);
				}
			}
		}

		public unsafe static void ConvertorFloatToShort2Channels(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)((void*)inputInterleavedBuffer);
			short* ptr2 = (short*)((void*)asioOutputBuffers[0]);
			short* ptr3 = (short*)((void*)asioOutputBuffers[1]);
			for (int i = 0; i < nbSamples; i++)
			{
				*(ptr2++) = AsioSampleConvertor.clampToShort((double)(*ptr));
				*(ptr3++) = AsioSampleConvertor.clampToShort((double)ptr[1]);
				ptr += 2;
			}
		}

		public unsafe static void ConvertorFloatToShortGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)((void*)inputInterleavedBuffer);
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)((void*)asioOutputBuffers[i]);
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					short*[] expr_36_cp_0 = array;
					int expr_36_cp_1 = k;
					short* ptr2 = expr_36_cp_0[expr_36_cp_1];
					expr_36_cp_0[expr_36_cp_1] = ptr2 + 1;
					*ptr2 = AsioSampleConvertor.clampToShort((double)(*(ptr++)));
				}
			}
		}

		public unsafe static void ConverterFloatTo24LSBGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)((void*)inputInterleavedBuffer);
			byte*[] array = new byte*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (byte*)((void*)asioOutputBuffers[i]);
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					int num = AsioSampleConvertor.clampTo24Bit((double)(*(ptr++)));
					byte*[] expr_44_cp_0 = array;
					int expr_44_cp_1 = k;
					byte* ptr2 = expr_44_cp_0[expr_44_cp_1];
					expr_44_cp_0[expr_44_cp_1] = ptr2 + 1;
					*ptr2 = (byte)num;
					byte*[] expr_5B_cp_0 = array;
					int expr_5B_cp_1 = k;
					ptr2 = expr_5B_cp_0[expr_5B_cp_1];
					expr_5B_cp_0[expr_5B_cp_1] = ptr2 + 1;
					*ptr2 = (byte)(num >> 8);
					byte*[] expr_74_cp_0 = array;
					int expr_74_cp_1 = k;
					ptr2 = expr_74_cp_0[expr_74_cp_1];
					expr_74_cp_0[expr_74_cp_1] = ptr2 + 1;
					*ptr2 = (byte)(num >> 16);
				}
			}
		}

		public unsafe static void ConverterFloatToFloatGeneric(IntPtr inputInterleavedBuffer, IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)((void*)inputInterleavedBuffer);
			float*[] array = new float*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (float*)((void*)asioOutputBuffers[i]);
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					float*[] expr_36_cp_0 = array;
					int expr_36_cp_1 = k;
					float* ptr2 = expr_36_cp_0[expr_36_cp_1];
					expr_36_cp_0[expr_36_cp_1] = ptr2 + 1;
					*ptr2 = *(ptr++);
				}
			}
		}

		private static int clampTo24Bit(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? -1.0 : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (int)(sampleValue * 8388607.0);
		}

		private static int clampToInt(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? -1.0 : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (int)(sampleValue * 2147483647.0);
		}

		private static short clampToShort(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? -1.0 : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (short)(sampleValue * 32767.0);
		}
	}
}
