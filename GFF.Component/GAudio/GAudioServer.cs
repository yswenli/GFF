using GFF.Component.GAudio.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GFF.Component.GAudio
{
    public class GAudioServer
    {
        AudioServer _audioServer;

        public GAudioServer(IPEndPoint endPoint)
        {
            _audioServer = new AudioServer(endPoint);
        }

        public GAudioServer(int port) : this(new IPEndPoint(IPAddress.Any, port))
        {

        }

        public void Start()
        {
            _audioServer.Start();
        }

        public void Stop()
        {
            _audioServer.Stop();
        }
    }
}
