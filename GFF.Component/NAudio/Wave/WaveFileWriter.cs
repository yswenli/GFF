using NAudio.Utils;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Text;

namespace GFF.Component.NAudio.Wave
{
	public class WaveFileWriter : Stream
	{
		private Stream outStream;

		private readonly BinaryWriter writer;

		private long dataSizePos;

		private long factSampleCountPos;

		private long dataChunkSize;

		private readonly WaveFormat format;

		private readonly string filename;

		private readonly byte[] value24 = new byte[3];

		public string Filename
		{
			get
			{
				return this.filename;
			}
		}

		public override long Length
		{
			get
			{
				return this.dataChunkSize;
			}
		}

		public WaveFormat WaveFormat
		{
			get
			{
				return this.format;
			}
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Position
		{
			get
			{
				return this.dataChunkSize;
			}
			set
			{
				throw new InvalidOperationException("Repositioning a WaveFileWriter is not supported");
			}
		}

		public static void CreateWaveFile16(string filename, ISampleProvider sourceProvider)
		{
			WaveFileWriter.CreateWaveFile(filename, new SampleToWaveProvider16(sourceProvider));
		}

		public static void CreateWaveFile(string filename, IWaveProvider sourceProvider)
		{
			using (WaveFileWriter waveFileWriter = new WaveFileWriter(filename, sourceProvider.WaveFormat))
			{
				byte[] array = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
				while (true)
				{
					int num = sourceProvider.Read(array, 0, array.Length);
					if (num == 0)
					{
						break;
					}
					waveFileWriter.Write(array, 0, num);
				}
			}
		}

		public static void WriteWavFileToStream(Stream outStream, IWaveProvider sourceProvider)
		{
			using (WaveFileWriter waveFileWriter = new WaveFileWriter(new IgnoreDisposeStream(outStream), sourceProvider.WaveFormat))
			{
				byte[] array = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
				while (true)
				{
					int num = sourceProvider.Read(array, 0, array.Length);
					if (num == 0)
					{
						break;
					}
					waveFileWriter.Write(array, 0, num);
				}
				outStream.Flush();
			}
		}

		public WaveFileWriter(Stream outStream, WaveFormat format)
		{
			this.outStream = outStream;
			this.format = format;
			this.writer = new BinaryWriter(outStream, Encoding.UTF8);
			this.writer.Write(Encoding.UTF8.GetBytes("RIFF"));
			this.writer.Write(0);
			this.writer.Write(Encoding.UTF8.GetBytes("WAVE"));
			this.writer.Write(Encoding.UTF8.GetBytes("fmt "));
			format.Serialize(this.writer);
			this.CreateFactChunk();
			this.WriteDataChunkHeader();
		}

		public WaveFileWriter(string filename, WaveFormat format) : this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), format)
		{
			this.filename = filename;
		}

		private void WriteDataChunkHeader()
		{
			this.writer.Write(Encoding.UTF8.GetBytes("data"));
			this.dataSizePos = this.outStream.Position;
			this.writer.Write(0);
		}

		private void CreateFactChunk()
		{
			if (this.HasFactChunk())
			{
				this.writer.Write(Encoding.UTF8.GetBytes("fact"));
				this.writer.Write(4);
				this.factSampleCountPos = this.outStream.Position;
				this.writer.Write(0);
			}
		}

		private bool HasFactChunk()
		{
			return this.format.Encoding != WaveFormatEncoding.Pcm && this.format.BitsPerSample != 0;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException("Cannot read from a WaveFileWriter");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException("Cannot seek within a WaveFileWriter");
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException("Cannot set length of a WaveFileWriter");
		}

		[Obsolete("Use Write instead")]
		public void WriteData(byte[] data, int offset, int count)
		{
			this.Write(data, offset, count);
		}

		public override void Write(byte[] data, int offset, int count)
		{
			if (this.outStream.Length + (long)count > (long)((ulong)-1))
			{
				throw new ArgumentException("WAV file too large", "count");
			}
			this.outStream.Write(data, offset, count);
			this.dataChunkSize += (long)count;
		}

		public void WriteSample(float sample)
		{
			if (this.WaveFormat.BitsPerSample == 16)
			{
				this.writer.Write((short)(32767f * sample));
				this.dataChunkSize += 2L;
				return;
			}
			if (this.WaveFormat.BitsPerSample == 24)
			{
				byte[] bytes = BitConverter.GetBytes((int)(2.14748365E+09f * sample));
				this.value24[0] = bytes[1];
				this.value24[1] = bytes[2];
				this.value24[2] = bytes[3];
				this.writer.Write(this.value24);
				this.dataChunkSize += 3L;
				return;
			}
			if (this.WaveFormat.BitsPerSample == 32 && this.WaveFormat.Encoding == WaveFormatEncoding.Extensible)
			{
				this.writer.Write(65535 * (int)sample);
				this.dataChunkSize += 4L;
				return;
			}
			if (this.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				this.writer.Write(sample);
				this.dataChunkSize += 4L;
				return;
			}
			throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
		}

		public void WriteSamples(float[] samples, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				this.WriteSample(samples[offset + i]);
			}
		}

		[Obsolete("Use WriteSamples instead")]
		public void WriteData(short[] samples, int offset, int count)
		{
			this.WriteSamples(samples, offset, count);
		}

		public void WriteSamples(short[] samples, int offset, int count)
		{
			if (this.WaveFormat.BitsPerSample == 16)
			{
				for (int i = 0; i < count; i++)
				{
					this.writer.Write(samples[i + offset]);
				}
				this.dataChunkSize += (long)(count * 2);
				return;
			}
			if (this.WaveFormat.BitsPerSample == 24)
			{
				for (int j = 0; j < count; j++)
				{
					byte[] bytes = BitConverter.GetBytes(65535 * (int)samples[j + offset]);
					this.value24[0] = bytes[1];
					this.value24[1] = bytes[2];
					this.value24[2] = bytes[3];
					this.writer.Write(this.value24);
				}
				this.dataChunkSize += (long)(count * 3);
				return;
			}
			if (this.WaveFormat.BitsPerSample == 32 && this.WaveFormat.Encoding == WaveFormatEncoding.Extensible)
			{
				for (int k = 0; k < count; k++)
				{
					this.writer.Write(65535 * (int)samples[k + offset]);
				}
				this.dataChunkSize += (long)(count * 4);
				return;
			}
			if (this.WaveFormat.BitsPerSample == 32 && this.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				for (int l = 0; l < count; l++)
				{
					this.writer.Write((float)samples[l + offset] / 32768f);
				}
				this.dataChunkSize += (long)(count * 4);
				return;
			}
			throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
		}

		public override void Flush()
		{
			long position = this.writer.BaseStream.Position;
			this.UpdateHeader(this.writer);
			this.writer.BaseStream.Position = position;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.outStream != null)
			{
				try
				{
					this.UpdateHeader(this.writer);
				}
				finally
				{
					this.outStream.Close();
					this.outStream = null;
				}
			}
		}

		protected virtual void UpdateHeader(BinaryWriter writer)
		{
			writer.Flush();
			this.UpdateRiffChunk(writer);
			this.UpdateFactChunk(writer);
			this.UpdateDataChunk(writer);
		}

		private void UpdateDataChunk(BinaryWriter writer)
		{
			writer.Seek((int)this.dataSizePos, SeekOrigin.Begin);
			writer.Write((uint)this.dataChunkSize);
		}

		private void UpdateRiffChunk(BinaryWriter writer)
		{
			writer.Seek(4, SeekOrigin.Begin);
			writer.Write((uint)(this.outStream.Length - 8L));
		}

		private void UpdateFactChunk(BinaryWriter writer)
		{
			if (this.HasFactChunk())
			{
				int num = this.format.BitsPerSample * this.format.Channels;
				if (num != 0)
				{
					writer.Seek((int)this.factSampleCountPos, SeekOrigin.Begin);
					writer.Write((int)(this.dataChunkSize * 8L / (long)num));
				}
			}
		}

		~WaveFileWriter()
		{
			this.Dispose(false);
		}
	}
}
