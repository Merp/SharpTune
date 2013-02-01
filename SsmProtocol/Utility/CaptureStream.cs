///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// CaptureStream.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace NateW.Ssm
{
    /// <summary>
    /// Wraps a given stream and records all reads and writes to a text file
    /// </summary>
    public class CaptureStream : Stream
    {
        /// <summary>
        /// The underlying stream
        /// </summary>
        private Stream stream;

        /// <summary>
        /// TextWriter for the capture
        /// </summary>
        private TextWriter log;

        private object writerLock = new object();

        //public Stream stream;
        //public TextWriter log;

        /// <summary>
        /// Passes straight through to the base stream
        /// </summary>
        public override bool CanRead
        {
            [DebuggerStepThrough()]
            get { return this.stream.CanRead; }
        }

        /// <summary>
        /// Passes straight through to the base stream
        /// </summary>
        public override bool CanSeek
        {
            [DebuggerStepThrough()]
            get { return this.stream.CanSeek; }
        }

        /// <summary>
        /// Passes straight through to the base stream
        /// </summary>
        public override bool CanWrite
        {
            [DebuggerStepThrough()]
            get { return this.stream.CanWrite; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream whose reads and writes will be recorded</param>
        /// <param name="log">Where to record the reads and writes</param>
        public CaptureStream(Stream stream, TextWriter log)
        {
            this.stream = stream;
            this.log = log;
        }

        /// <summary>
        /// Records the operation and passes through to the base stream
        /// </summary>
        public override void Flush()
        {
            this.log.WriteLine("Flush");
            this.log.Flush();
            this.stream.Flush();
        }

        /// <summary>
        /// Passes stright through to the base stream
        /// </summary>
        public override long Length
        {
            get { return this.stream.Length; }
        }

        /// <summary>
        /// Records set operations, passes stright through to the base stream
        /// </summary>
        public override long Position
        {
            get
            {
                return this.stream.Position;
            }
            set
            {
                this.log.WriteLine("set_Position " + value.ToString(CultureInfo.InvariantCulture));
                this.log.Flush();
                this.stream.Position = value;
            }
        }

        /// <summary>
        /// Records the data read, passes through to the base stream
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = 0;
            try
            {
                System.Threading.Thread.Sleep(count);
                result = this.stream.Read(buffer, offset, count);
            }
            catch (TimeoutException)
            {
                this.log.Write('*');
            }

            this.WriteBuffer("Read ", buffer, offset, result);
            return result;
        }

        /// <summary>
        /// Records the operation and passes through to the base stream
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long result = this.stream.Seek(offset, origin);
            this.log.WriteLine("Seek (" + offset.ToString(CultureInfo.InvariantCulture) + ", " + origin.ToString());
            this.log.Flush();
            return result;
        }

        /// <summary>
        /// Records the operation and passes through to the base stream
        /// </summary>
        public override void SetLength(long value)
        {
            this.log.WriteLine("SetLength " + value);
            this.stream.SetLength(value);
        }

        /// <summary>
        /// Records the data written and passes through to the base stream
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.WriteBuffer("Write", buffer, offset, count);
            this.stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Dumps a buffer to the log
        /// </summary>
        private void WriteBuffer(string message, byte[] buffer, int offset, int count)
        {
            lock (this.writerLock)
            {
                this.log.Write(message + " ");

                for (int i = 0; i < count; i++)
                {
                    byte value = buffer[offset + i];
                    this.log.Write("0x" + value.ToString("X02", CultureInfo.InvariantCulture) + ", ");
                }

                this.log.WriteLine();
                this.log.Flush();
            }
        }
    }
}
