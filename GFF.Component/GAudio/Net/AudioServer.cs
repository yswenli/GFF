using SAEA.Sockets;
using SAEA.Sockets.Base;
using SAEA.Sockets.Interface;
using SAEA.Sockets.Model;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace GFF.Component.GAudio.Net
{
    public class AudioServer
    {
        IServerSocket _udpServer;

        ConcurrentDictionary<string, IUserToken> _cache;

        public AudioServer(IPEndPoint endPoint)
        {
            _cache = new ConcurrentDictionary<string, IUserToken>();

            _udpServer = SocketFactory.CreateServerSocket(SocketOptionBuilder.Instance.SetSocket(SAEASocketType.Udp)
                .SetIPEndPoint(endPoint)
                .UseIocp<BaseContext>()
                .SetReadBufferSize(SocketOption.UDPMaxLength)
                .SetWriteBufferSize(SocketOption.UDPMaxLength)
                .SetTimeOut(5000)
                .Build());
            _udpServer.OnAccepted += _udpServer_OnAccepted;
            _udpServer.OnDisconnected += _udpServer_OnDisconnected;
            _udpServer.OnReceive += _udpServer_OnReceive;
        }

        public void Start()
        {
            _udpServer.Start();
        }

        public void Stop()
        {
            _udpServer.Stop();
        }

        private void _udpServer_OnReceive(ISession currentSession, byte[] data)
        {
            var userToken = (IUserToken)currentSession;

            Parallel.ForEach(_cache.Keys, (id) =>
            {
                try
                {
                    if (id != userToken.ID)
                        _udpServer.SendAsync(id, data);
                }
                catch { }
            });
        }

        private void _udpServer_OnAccepted(object obj)
        {
            var ut = (IUserToken)obj;
            if (ut != null)
            {
                _cache.TryAdd(ut.ID, ut);
            }
        }

        private void _udpServer_OnDisconnected(string ID, Exception ex)
        {
            _cache.TryRemove(ID, out IUserToken _);
        }
    }
}
