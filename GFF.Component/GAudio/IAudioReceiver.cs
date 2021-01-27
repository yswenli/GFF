using System;

namespace GFF.Component.GAudio
{
    interface IAudioReceiver : IDisposable
    {
        void OnReceived(Action<byte[]> handler);
    }
}