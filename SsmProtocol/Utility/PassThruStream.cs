using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NateW.J2534;

namespace NateW.Ssm
{
    /// <summary>
    /// Implements Stream API atop PassThru API.  
    /// </summary>
    /// <remarks>
    /// Has some SSM/ISO9141 details baked in - will need changes to support other protocols.
    /// </remarks>
    internal class PassThruStream : Stream
    {
        private IPassThru passThru;
        private PassThruDevice device;
        private PassThruChannel channel;
        private byte[] received;

        private PassThruStream(string passThruDllPath)
        {
            if (passThruDllPath == "Mock")
            {
                this.passThru = new MockPassThru();
            }
            else
            {
                this.passThru = DynamicPassThru.GetInstance(passThruDllPath);
            }
        }

        public static PassThruStream GetInstance(string passThruDllPath)
        {
            return new PassThruStream(passThruDllPath);
        }

        public void OpenSsmChannel()
        {
            Trace.WriteLine("Opening PassThru device.");
            this.device = PassThruDevice.GetInstance(this.passThru);
            device.Open();

            Trace.WriteLine("Opening PassThru channel.");
            this.channel = device.OpenChannel(
                PassThruProtocol.Iso9141,
                PassThruConnectFlags.Iso9141NoChecksum,
                PassThruBaudRate.Rate4800);

            Trace.WriteLine("Initializing channel for SSM.");
            channel.InitializeSsm();
        }

        public override bool CanRead
        {
            get { return this.channel != null; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return this.channel != null; }
        }

        public override void Flush()
        {
            return;
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.channel == null)
            {
                throw new InvalidOperationException("PassThruStream.OpenSsmStream() must succeed before calling PassThruStream.Read()");
            }

            if (this.received != null)
            {
                int bytesToCopy = Math.Min(count, this.received.Length);
                for (int i = 0; i < bytesToCopy; i++)
                {
                    buffer[i + offset] = this.received[i];
                }

                int remaining = this.received.Length - bytesToCopy;
                if (remaining > 0)
                {
                    byte[] newBuffer = new byte[remaining];
                    for (int i = 0; i < remaining; i++)
                    {
                        newBuffer[i] = this.received[bytesToCopy + i];
                    }
                    this.received = newBuffer;
                }
                else
                {
                    this.received = null;
                }
                return bytesToCopy;
            }
            else
            {
                PassThruMsg message = new PassThruMsg();
                this.channel.ReadMessage(message, TimeSpan.FromSeconds(0.5));
                int bytesToCopy = (int) Math.Min(count, message.DataSize);
                for (int i = 0; i < bytesToCopy; i++)
                {
                    buffer[i+offset] = message.Data[i];
                }

                if (bytesToCopy < message.DataSize)
                {
                    this.received = new byte[message.DataSize - bytesToCopy];
                    for (int i = 0; i < (message.DataSize - bytesToCopy); i++)
                    {
                        this.received[i] = message.Data[bytesToCopy + i];
                    }
                }
                return bytesToCopy;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.channel == null)
            {
                throw new InvalidOperationException("PassThruStream.OpenSsmStream() must succeed before calling PassThruStream.Read()");
            }

            int written = 0;
            while (written < count)
            {
                PassThruMsg message = new PassThruMsg();
                message.ProtocolID = PassThruProtocol.Iso9141;

                int length = Math.Min(count, message.Data.Length);
                for (int i = 0; i < length; i++)
                {
                    message.Data[i] = buffer[offset + i];
                }
                message.DataSize = (uint) length;

                this.channel.WriteMessage(message, TimeSpan.FromSeconds(0.5));
                written += length;
            }
        }
    }
}
