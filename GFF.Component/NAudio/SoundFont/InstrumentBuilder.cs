using System;
using System.IO;
using System.Text;

namespace GFF.Component.NAudio.SoundFont
{
	internal class InstrumentBuilder : StructureBuilder<Instrument>
	{
		private Instrument lastInstrument;

		public override int Length
		{
			get
			{
				return 22;
			}
		}

		public Instrument[] Instruments
		{
			get
			{
				return this.data.ToArray();
			}
		}

		public override Instrument Read(BinaryReader br)
		{
			Instrument instrument = new Instrument();
			string text = Encoding.UTF8.GetString(br.ReadBytes(20), 0, 20);
			if (text.IndexOf('\0') >= 0)
			{
				text = text.Substring(0, text.IndexOf('\0'));
			}
			instrument.Name = text;
			instrument.startInstrumentZoneIndex = br.ReadUInt16();
			if (this.lastInstrument != null)
			{
				this.lastInstrument.endInstrumentZoneIndex = instrument.startInstrumentZoneIndex - 1;
			}
			this.data.Add(instrument);
			this.lastInstrument = instrument;
			return instrument;
		}

		public override void Write(BinaryWriter bw, Instrument instrument)
		{
		}

		public void LoadZones(Zone[] zones)
		{
			for (int i = 0; i < this.data.Count - 1; i++)
			{
				Instrument instrument = this.data[i];
				instrument.Zones = new Zone[(int)(instrument.endInstrumentZoneIndex - instrument.startInstrumentZoneIndex + 1)];
				Array.Copy(zones, (int)instrument.startInstrumentZoneIndex, instrument.Zones, 0, instrument.Zones.Length);
			}
			this.data.RemoveAt(this.data.Count - 1);
		}
	}
}
