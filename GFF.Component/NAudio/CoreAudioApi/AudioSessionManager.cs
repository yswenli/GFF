using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GFF.Component.NAudio.CoreAudioApi
{
	public class AudioSessionManager
	{
		public delegate void SessionCreatedDelegate(object sender, IAudioSessionControl newSession);

		private readonly IAudioSessionManager audioSessionInterface;

		private readonly IAudioSessionManager2 audioSessionInterface2;

		private AudioSessionNotification audioSessionNotification;

		private SessionCollection sessions;

		private SimpleAudioVolume simpleAudioVolume;

		private AudioSessionControl audioSessionControl;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event AudioSessionManager.SessionCreatedDelegate OnSessionCreated;

		public SimpleAudioVolume SimpleAudioVolume
		{
			get
			{
				if (this.simpleAudioVolume == null)
				{
					ISimpleAudioVolume realSimpleVolume;
					this.audioSessionInterface.GetSimpleAudioVolume(Guid.Empty, 0u, out realSimpleVolume);
					this.simpleAudioVolume = new SimpleAudioVolume(realSimpleVolume);
				}
				return this.simpleAudioVolume;
			}
		}

		public AudioSessionControl AudioSessionControl
		{
			get
			{
				if (this.audioSessionControl == null)
				{
					IAudioSessionControl audioSessionControl;
					this.audioSessionInterface.GetAudioSessionControl(Guid.Empty, 0u, out audioSessionControl);
					this.audioSessionControl = new AudioSessionControl(audioSessionControl);
				}
				return this.audioSessionControl;
			}
		}

		public SessionCollection Sessions
		{
			get
			{
				return this.sessions;
			}
		}

		internal AudioSessionManager(IAudioSessionManager audioSessionManager)
		{
			this.audioSessionInterface = audioSessionManager;
			this.audioSessionInterface2 = (audioSessionManager as IAudioSessionManager2);
			this.RefreshSessions();
		}

		internal void FireSessionCreated(IAudioSessionControl newSession)
		{
			AudioSessionManager.SessionCreatedDelegate expr_06 = this.OnSessionCreated;
			if (expr_06 == null)
			{
				return;
			}
			expr_06(this, newSession);
		}

		public void RefreshSessions()
		{
			this.UnregisterNotifications();
			if (this.audioSessionInterface2 != null)
			{
				IAudioSessionEnumerator realEnumerator;
				Marshal.ThrowExceptionForHR(this.audioSessionInterface2.GetSessionEnumerator(out realEnumerator));
				this.sessions = new SessionCollection(realEnumerator);
				this.audioSessionNotification = new AudioSessionNotification(this);
				Marshal.ThrowExceptionForHR(this.audioSessionInterface2.RegisterSessionNotification(this.audioSessionNotification));
			}
		}

		public void Dispose()
		{
			this.UnregisterNotifications();
			GC.SuppressFinalize(this);
		}

		private void UnregisterNotifications()
		{
			this.sessions = null;
			if (this.audioSessionNotification != null && this.audioSessionInterface2 != null)
			{
				Marshal.ThrowExceptionForHR(this.audioSessionInterface2.UnregisterSessionNotification(this.audioSessionNotification));
				this.audioSessionNotification = null;
			}
		}

		~AudioSessionManager()
		{
			this.Dispose();
		}
	}
}
