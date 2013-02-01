using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NateW.J2534;

namespace NateW.Ssm
{
    internal abstract class SsmTransport
    {
        public abstract void Initialize();

        public abstract IAsyncResult BeginSend(SsmPacket packet, AsyncCallback callback, object state);
        public abstract void EndSend(IAsyncResult asyncResult);

        public abstract IAsyncResult BeginReceive(AsyncCallback callback, object state);
        public abstract SsmPacket EndReceive(AsyncResult);
    }

    internal class TransportReceiveAsyncResult : AsyncResult
    {
        private SsmPacket packet;

        public SsmPacket Packet { get { return this.packet; } }

        public TransportReceiveAsyncResult()
        {
        }
    }

    internal class SsmSerialTransport
    {
        private Stream stream;

        private SsmSerialTransport(Stream stream)
        {
            this.stream = stream;
        }

        public static SsmTransport GetInstance(Stream stream)
        {
            return new SsmSerialTransport(stream);
        }

        public void Initialize()
        {
        }

        public IAsyncResult BeginSend(SsmPacket packet, AsyncCallback callback, object state)
        {
            this.stream.BeginWrite(
                packet.Data, 
                0, 
                packet.Data.Length, 
                callback,
                internalState);
            return internalState;
        }

        public void EndSend(IAsyncResult asyncResult)
        {
            this.stream.EndWrite(asyncResult);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
        }

        public SsmPacket EndReceive(AsyncResult)
        {
        }
    }

    internal class SsmPassThruTransport
    {
        private IPassThru passThru;
        private int deviceId;
        private int channelId;

        private SsmPassThruTransport(string passThruDllPath)
        {
            this.passThru = DynamicPassThru.GetInstance(passThruDllPath);
        }

        public static SsmTransport GetInstance(string passThruDllPath)
        {
            return new SsmPassThruTransport(passThruDllPath);
        }

        public void Initialize()
        {
        }

        public IAsyncResult BeginSend(SsmPacket packet, AsyncCallback callback, object state)
        {
        }

        public void EndSend(IAsyncResult asyncResult)
        {
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
        }

        public SsmPacket EndReceive(AsyncResult)
        {
        }
    }
}
