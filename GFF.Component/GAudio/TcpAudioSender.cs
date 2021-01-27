using System;
using System.Net;
using System.Net.Sockets;

namespace GFF.Component.GAudio
{
    class TcpAudioSender : IAudioSender
    {
        private readonly TcpClient tcpSender;
        public TcpAudioSender(IPEndPoint endPoint)
        {
            tcpSender = new TcpClient();
            tcpSender.Connect(endPoint);
        }

        public void Send(byte[] payload)
        {
            tcpSender.Client.Send(payload);
        }

        public void Dispose()
        {
            tcpSender?.Close();
        }
    }
}