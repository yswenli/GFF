using GFF.Component.GAudio.Net;
using GFF.Component.NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GFF.Component.GAudio
{
    public class GAudioClient
    {
        AudioClient _audioClient;

        private readonly IWavePlayer waveOut;
        private readonly BufferedWaveProvider waveProvider;
        private readonly WideBandSpeexCodec _speexCodec;
        private readonly WaveIn waveIn;

        public GAudioClient(IPEndPoint endPoint)
        {
            _audioClient = new AudioClient(endPoint);
            _audioClient.OnReceive += _audioClient_OnReceive;

            _speexCodec = new WideBandSpeexCodec();

            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(_speexCodec.RecordFormat);
            waveOut.Init(waveProvider);


            waveIn = new WaveIn();
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = _speexCodec.RecordFormat;
            waveIn.DataAvailable += OnAudioCaptured;
        }

        public GAudioClient(string ip, int port) : this(new IPEndPoint(IPAddress.Parse(ip), port))
        {

        }

        public void Start()
        {
            _audioClient.Connect();

            waveOut.Play();

            waveIn.StartRecording();
        }

        private void _audioClient_OnReceive(byte[] data)
        {
            byte[] decoded = _speexCodec.Decode(data, 0, data.Length);
            waveProvider.AddSamples(decoded, 0, decoded.Length);
        }

        void OnAudioCaptured(object sender, WaveInEventArgs e)
        {
            byte[] encoded = _speexCodec.Encode(e.Buffer, 0, e.BytesRecorded);
            _audioClient.Send(encoded);
        }


        public void Stop()
        {
            waveIn.StopRecording();
            waveOut.Pause();
            _audioClient.Disconnect();
        }

        public void Dispose()
        {
            Stop();
            waveIn.Dispose();
            waveOut.Dispose();            
        }
    }
}
