using System;

namespace GFF.Component.GAudio
{
    interface IAudioSender : IDisposable
    {
        void Send(byte[] payload);
    }
}