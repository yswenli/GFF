using SAEA.Sockets;
using SAEA.Sockets.Base;
using SAEA.Sockets.Model;
using System;
using System.Net;

namespace GFF.Component.GAudio.Net
{
    public class AudioClient
    {
        IClientSocket _udpClient;

        BaseUnpacker _baseUnpacker;

        public event Action<Byte[]> OnReceive;

        public AudioClient(IPEndPoint endPoint)
        {
            var bContext = new BaseContext();

            _udpClient = SocketFactory.CreateClientSocket(SocketOptionBuilder.Instance.SetSocket(SAEASocketType.Udp)
                .SetIPEndPoint(endPoint)
                .UseIocp(bContext)
                .SetReadBufferSize(SocketOption.UDPMaxLength)
                .SetWriteBufferSize(SocketOption.UDPMaxLength)
                .Build());

            _baseUnpacker = (BaseUnpacker)bContext.Unpacker;

            _udpClient.OnReceive += _udpClient_OnReceive;
        }

        private void _udpClient_OnReceive(byte[] data)
        {
            OnReceive?.Invoke(data);
        }

        public void Connect()
        {
            _udpClient.Connect();
        }

        public void Send(byte[] data)
        {
            _udpClient.SendAsync(data);
        }

        public void Disconnect()
        {
            _udpClient.Disconnect();
        }

    }
}
