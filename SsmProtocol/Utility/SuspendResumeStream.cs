using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    public delegate Stream StreamFactory();

    /// <summary>
    /// Stream class that closes the underlying stream while the computer is suspended
    /// TODO: BeginRead / EndRead - how do you know it's still the same stream?  Need to wrap the IAsyncResult.
    /// TODO: maintain count of current async operations, don't re-create until it reaches zero?
    /// TODO: atomic state transitions (timer used for OS resume, retrying stream creation)
    /// </summary>
    class SuspendResumeStream : Stream
    {
        private enum State
        {
            Initial,
            Active,
            Suspended,
            Resuming
        }

        private StreamFactory factory;
        private Stream activeStream;
        private object syncObject;
        private object stateLock;
        private State state;
        
        /// <summary>
        /// System hangs if serial activity restarts too quickly after a resume from hibernation.
        /// </summary>
        private Timer resumeTimer;

        /// <summary>
        /// This is kept as a member so the handler can be unregistered during dispose
        /// </summary>
        private Microsoft.Win32.PowerModeChangedEventHandler powerModeChangedEventHandler;

        /// <summary>
        /// For use by SuspendResumeStream.Synchronizer only.
        /// </summary>
        internal object SyncObject
        {
            get { return this.syncObject; }
        }

        /// <summary>
        /// Synchronizes access to operations on the underlying stream.
        /// </summary>
        private struct Synchronizer : IDisposable
        {
            private SuspendResumeStream stream;

            public Synchronizer(SuspendResumeStream stream)
            {
                this.stream = stream;
                Monitor.Enter(this.stream.SyncObject);
            }

            public void Dispose()
            {
                Monitor.Exit(this.stream.SyncObject);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory">Factory method to create underlying stream.</param>
        public SuspendResumeStream(StreamFactory factory)
        {
            this.syncObject = new object();
            this.stateLock = new object();
            this.state = State.Initial;
            this.factory = factory;

            this.powerModeChangedEventHandler = new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            Microsoft.Win32.SystemEvents.PowerModeChanged += this.powerModeChangedEventHandler;

            this.TryStateTransition(State.Active, this.Open);
        }

        #region Stream overrides 

        public override bool CanRead
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.CanRead;
                }
            }
        }

        public override bool CanSeek
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.CanSeek;
                }
            }
        }

        public override bool CanWrite
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.CanWrite;
                }
            }
        }

        public override void Flush()
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                stream.Flush();
            }            
        }

        public override long Length
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.Length;
                }
            }
        }

        public override long Position
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.Position;
                }
            }

            set
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    stream.Position = value;
                }
            }
        }

        public override bool CanTimeout
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.CanTimeout;
                }
            }
        }

        public override int ReadTimeout
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.ReadTimeout;
                }
            }

            set
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    stream.ReadTimeout = value;
                }

            }
        }

        public override int WriteTimeout
        {
            get
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    return stream.WriteTimeout;
                }
            }

            set
            {
                using (new Synchronizer(this))
                {
                    Stream stream = this.GetStream();
                    stream.WriteTimeout = value;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                return stream.Read(buffer, offset, count);
            }
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                return stream.Seek(offset, origin);
            }
        }

        public override void SetLength(long value)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                stream.SetLength(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                stream.Write(buffer, offset, count);
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object asyncState)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                return stream.BeginRead(buffer, offset, count, callback, asyncState);
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                return stream.EndRead(asyncResult);
            }
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object asyncState)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                return stream.BeginWrite(buffer, offset, count, callback, asyncState);
            }
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                stream.EndWrite(asyncResult);
            }
        }

        public new void Dispose()
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.activeStream;
                if (stream != null)
                {
                    stream.Dispose();
                    this.activeStream = null;
                }
            }

            if (this.resumeTimer != null)
            {
                this.resumeTimer.Dispose();
                this.resumeTimer = null;
            }

            GC.SuppressFinalize(this);
        }

        public override void Close()
        {
            using (new Synchronizer(this))
            {
                Stream stream = this.GetStream();
                stream.Close();
            }
        }
        #endregion

        /// <summary>
        /// This simply invokes the consumer's stream factory.
        /// </summary>
        public void Open()
        {
            // ...but do not return the underlying stream.
            Trace.WriteLine("SuspendResumeStream.Open");
            this.GetStream();
        }

        public override string ToString()
        {
            Stream local = this.activeStream;
            string streamDescription = null;
            if (local != null)
            {
                streamDescription = local.ToString();
            }

            return "SuspendResumStream, state = " + this.state.ToString() + ", stream = " + streamDescription;
        }

        /// <summary>
        /// Stop/start logging when the system hibernates and resumes
        /// </summary>
        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    Trace.WriteLine("SuspendResumeStream.PowerModeChanged: Suspend");
                    this.Suspend();
                    break;

                case Microsoft.Win32.PowerModes.Resume:
                    // Must sleep a while or the serial port won't open properly
                    Trace.WriteLine("SuspendResumeStream.PowerModeChanged: Resume");
                    this.Resume();
                    break;
            }
        }

        /// <summary>
        /// Suspend serial activity - for use by private and test code only.
        /// </summary>
        internal void Suspend()
        {
            Trace.WriteLine("### SuspendResumeStream: PowerModeChanged: Suspend");
            if (!this.TryStateTransition(State.Suspended, this.InternalSuspend))
            {
                Trace.WriteLine("SuspendResumeStream.Resume: bad transition.");
                return;
            }
        }

        /// <summary>
        /// Dispose the stream, cancelling pending operations.
        /// </summary>
        private void InternalSuspend()
        {
            Trace.WriteLine("SuspendResumeStream.InternalSuspend");
            using (new Synchronizer(this))
            {
                this.activeStream.Dispose();
                this.activeStream = null;
            }
        }

        /// <summary>
        /// Resume serial activity - for use by private and test code only.
        /// </summary>
        internal void Resume()
        {
            Trace.WriteLine("### SuspendResumeStream: PowerModeChanged: Resume");
            if (!this.TryStateTransition(State.Resuming, this.SetTimer))
            {
                Trace.WriteLine("SuspendResumeStream.Resume: bad transition.");
                return;
            }
        }

        /// <summary>
        /// Set a resumeTimer to resume serial activity later.
        /// </summary>
        private void SetTimer()
        {
            Trace.WriteLine("SuspendResumeStream.SetTimer");
            this.resumeTimer = new Timer(
                this.InternalResume,
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Restart logging after a system resume
        /// </summary>
        private void InternalResume(object unused)
        {
            Trace.WriteLine("SuspendResumeStream.InternalResume: State is " + this.state.ToString());
            this.resumeTimer = null;
            this.TryStateTransition(State.Active, this.Open);
        }

        /// <summary>
        /// Switch states, log the transition, and execute an operation that 
        /// must be atomic with the state change.
        /// </summary>
        private bool TryStateTransition(State newState, VoidVoid atomicOperation)
        {
            lock (this.stateLock)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "SuspendResumeStream.TryStateTransition: from {0} to {1} on thread {2}",
                    this.state,
                    newState,
                    Thread.CurrentThread.ManagedThreadId);
                Trace.WriteLine(message);

                switch (newState)
                {
                    case State.Initial:
                        throw new InvalidOperationException("Should never transition to state Initial.");

                    case State.Active:
                        if (this.state != State.Resuming && this.state != State.Initial)
                        {
                            Trace.WriteLine("SuspendResumeStream is neither Initial nor Resuming");
                            return false;
                        }
                        break;

                    case State.Resuming:
                        if (this.state != State.Suspended)
                        {
                            Trace.WriteLine("SuspendResumeStream is not suspended.");
                            return false;
                        }
                        break;

                    case State.Suspended:
                        if (this.state != State.Active)
                        {
                            Trace.WriteLine("SuspendResumeStream is not active.");
                            return false;
                        }
                        break;
                }

                Trace.WriteLine("SsmLogger.TryStateTransition: legal transition.");
                this.state = newState;
                if (atomicOperation != null)
                {
                    atomicOperation();
                }
            }
            return true;
        }

        /// <summary>
        /// Invoke the consumer's stream factory.
        /// </summary>
        private Stream GetStream()
        {
            if (this.state != State.Active)
            {
                throw new IOException("Stream closed, we're not active.");
            }

            if (this.activeStream == null)
            {
                try
                {
                    this.activeStream = this.factory();
                }
                catch (IOException ex)
                {
                    Trace.WriteLine("SuspendResumeStream.GetStream: stream factory threw " + ex.ToString());
                    if (!this.TryStateTransition(State.Resuming, this.SetTimer))
                    {
                        Trace.WriteLine("SuspendResumeStream.GetStream: bad transition.");
                    }
                    Trace.WriteLine("SuspendResumeStream.GetStream: rethrowing.");
                    throw;
                }
            }

            return this.activeStream;
        }
    }
}
