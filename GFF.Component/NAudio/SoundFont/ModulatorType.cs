using System;

namespace GFF.Component.NAudio.SoundFont
{
	public class ModulatorType
	{
		private bool polarity;

		private bool direction;

		private bool midiContinuousController;

		private ControllerSourceEnum controllerSource;

		private SourceTypeEnum sourceType;

		private ushort midiContinuousControllerNumber;

		internal ModulatorType(ushort raw)
		{
			this.polarity = ((raw & 512) == 512);
			this.direction = ((raw & 256) == 256);
			this.midiContinuousController = ((raw & 128) == 128);
			this.sourceType = (SourceTypeEnum)((raw & 64512) >> 10);
			this.controllerSource = (ControllerSourceEnum)(raw & 127);
			this.midiContinuousControllerNumber = (raw & 127);
		}

		public override string ToString()
		{
			if (this.midiContinuousController)
			{
				return string.Format("{0} CC{1}", this.sourceType, this.midiContinuousControllerNumber);
			}
			return string.Format("{0} {1}", this.sourceType, this.controllerSource);
		}
	}
}
