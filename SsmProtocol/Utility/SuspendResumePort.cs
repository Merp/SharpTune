using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    public delegate SerialPort SuspendResumePortFactory();
    public delegate void SuspendResumePortOperation(Stream stream);
    
    /// <summary>
    /// Continuously writes queries to / reads responses from a serial port, handling system suspend/resume.
    /// </summary>
    class SuspendResumePort : IDisposable
    {
        private enum State
        {
            Initial,
            Active,
            Restarting,
            Suspended,
            Resuming,
            Closed
        }

        private SerialPort port;
        private SuspendResumePortFactory factory;
        private SuspendResumePortOperation operation;
        private object stateLock;
        private State state;
        private int opened;

        /// <summary>
        /// System hangs if serial activity restarts too quickly after a resume from hibernation.
        /// </summary>
        private Timer resumeTimer;

        /// <summary>
        /// This is kept as a member so the handler can be unregistered during dispose
        /// </summary>
        private Microsoft.Win32.PowerModeChangedEventHandler powerModeChangedEventHandler;

        public SerialPort Port
        {
            get { return this.port; }
        }

        /// <summary>
        /// For test use.
        /// </summary>
        internal int Opened
        {
            get { return this.opened; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory">Factory method to create underlying stream.</param>
        /// <param name="operation">Method to begin reading from (or writing to) the stream.</param>
        public SuspendResumePort(
            SuspendResumePortFactory factory,
            SuspendResumePortOperation operation)
        {
            this.stateLock = new object();
            this.state = State.Initial;
            this.factory = factory;
            this.operation = operation;

            this.powerModeChangedEventHandler = new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            Microsoft.Win32.SystemEvents.PowerModeChanged += this.powerModeChangedEventHandler;
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~SuspendResumePort()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose the Timer.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        /// <summary>
        /// Invoke the consumer's factory, set timer and try again if factory fails.
        /// </summary>
        public void Open()
        {
            Trace.WriteLine("SuspendResumePort.Open");
            if (this.TryStateTransition(State.Active, this.InternalOpen))
            {
                if (this.port != null)
                {
                    this.opened++;
                    Trace.WriteLine("### SuspendResumePort.Open succeeded.");
                    this.StartOperation();
                }
                else
                {
                    this.Restart();
                }
            }
        }

        /// <summary>
        /// Invoke the consumer's BeginRead / BeginWrite to get the ball rolling.
        /// </summary>
        public void StartOperation()
        {
            Stream stream;
            if (this.TryGetBaseStream(out stream))
            {
                try
                {
                    this.operation(stream);
                }
                catch (IOException ex)
                {
                    Trace.WriteLine("SuspendResumePort.StartOperation: " + ex.Message);
                    this.Restart();
                }
            }
            else
            {
                Trace.WriteLine("SuspendResumePort.StartOperation: TryGetBaseStream failed.");
            }
        }

        /// <summary>
        /// No more opens or resumes.
        /// </summary>
        public void Close()
        {
            Trace.WriteLine("SuspendResumePort.Close: " + this.factory.ToString());

            // This transition is always legal and need not be atomic.
            this.state = State.Closed;
            this.port.Dispose();
        }

        /// <summary>
        /// Set timer to a short period (1s) and restart.
        /// </summary>
        public void Restart()
        {
            // Set a short timer to resume.
            if (!this.TryStateTransition(
                State.Restarting,
                delegate { this.SetTimer(TimeSpan.FromSeconds(1)); }))
            {
                Trace.WriteLine("### SuspendResumePort.Restart could not transition to Restarting");
            }
        }

        /// <summary>
        /// For debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "SuspendResumePort, state = " + this.state.ToString();
        }

        /// <summary>
        /// Dispose the Timer.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.resumeTimer != null)
                {
                    this.resumeTimer.Dispose();
                }
            }
        }

        /// <summary>
        /// Try to get the base Stream from the current port.
        /// </summary>
        private bool TryGetBaseStream(out Stream stream)
        {
            SerialPort localPort = this.port;
            if (localPort == null)
            {
                Trace.WriteLine("SuspendResumePort.TryGetBaseStream: this.port is null");
                stream = null;
                return false;
            }

            try
            {
                stream = localPort.BaseStream;
                return true;
            }
            catch (System.InvalidOperationException ex)
            {
                Trace.WriteLine("SuspendResumePort.TryGetBaseStream: " + ex.Message);
                stream = null;
                return false;
            }
        }

        /// <summary>
        /// Stop/start logging when the system hibernates and resumes
        /// </summary>
        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    Trace.WriteLine("SuspendResumePort.PowerModeChanged: Suspend");
                    this.Suspend();
                    break;

                case Microsoft.Win32.PowerModes.Resume:
                    // Must sleep a while or the serial port won't open properly
                    Trace.WriteLine("SuspendResumePort.PowerModeChanged: Resume");
                    this.Resume(TimeSpan.FromSeconds(10));
                    break;
            }
        }

        /// <summary>
        /// Suspend serial activity - for use by private and test code only.
        /// </summary>
        internal void Suspend()
        {
            Trace.WriteLine("### SuspendResumePort: PowerModeChanged: Suspend");
            if (!this.TryStateTransition(State.Suspended, this.InternalSuspend))
            {
                Trace.WriteLine("SuspendResumePort.Resume: bad transition.");
                return;
            }
        }

        /// <summary>
        /// Dispose the stream, cancelling pending operations.
        /// </summary>
        private void InternalSuspend()
        {
            this.port.Dispose();
            this.port = null;
        }

        /// <summary>
        /// Resume serial activity - for use by private and test code only.
        /// </summary>
        internal void Resume(TimeSpan interval)
        {
            Trace.WriteLine("### SuspendResumePort: PowerModeChanged: Resume");
            if (!this.TryStateTransition(
                State.Resuming,
                delegate { this.SetTimer(interval); }))
            {
                Trace.WriteLine("SuspendResumePort.Resume: bad transition.");
                return;
            }
        }

        /// <summary>
        /// Set a resumeTimer to resume serial activity later.
        /// </summary>
        private void SetTimer(TimeSpan interval)
        {
            Trace.WriteLine("SuspendResumePort.SetTimer");
            this.resumeTimer = new Timer(
                this.TimerTick,
                null,
                interval,
                TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Restart logging after a system resume
        /// </summary>
        private void TimerTick(object unused)
        {
            Trace.WriteLine("SuspendResumePort.TimerTick: State is " + this.state.ToString());
            this.resumeTimer = null;
            this.Open();
        }

        /// <summary>
        /// This simply invokes the consumer's port factory.
        /// </summary>
        private void InternalOpen()
        {
            Trace.WriteLine("SuspendResumePort.InternalOpen");
            try
            {
                this.port = this.factory();
            }
            catch (System.UnauthorizedAccessException exception)
            {
                Trace.WriteLine("SuspendResumePort.InternalOpen: " + exception.Message);
            }
            catch (System.IO.IOException exception)
            {
                Trace.WriteLine("SuspendResumePort.InternalOpen: " + exception.Message);
            }
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
                    "SuspendResumePort.TryStateTransition: from {0} to {1} on thread {2}",
                    this.state,
                    newState,
                    Thread.CurrentThread.ManagedThreadId);
                Trace.WriteLine(message);

                switch (newState)
                {
                    case State.Initial:
                        throw new InvalidOperationException("Should never transition to state Initial.");

                    case State.Active:
                        if (this.state != State.Resuming &&
                            this.state != State.Restarting &&
                            this.state != State.Initial)
                        {
                            Trace.WriteLine("SuspendResumePort.TryStateTransition: neither Initial, Resuming, nor Restarting.");
                            return false;
                        }
                        break;

                    case State.Restarting:
                        if (this.state != State.Active)
                        {
                            Trace.WriteLine("SuspendResumePort.TryStateTransition: not active, aborting the restart attempt.");
                            return false;
                        }
                        break;

                    case State.Resuming:
                        if (this.state != State.Suspended)
                        {
                            Trace.WriteLine("SuspendResumePort.TryStateTransition: not suspended.");
                            return false;
                        }
                        break;

                    case State.Suspended:
                        if (this.state != State.Active &&
                            this.state != State.Restarting &&
                            this.state != State.Resuming)
                        {
                            Trace.WriteLine("SuspendResumePort.TryStateTransition: not active, restarting, or resuming.");
                            return false;
                        }
                        // Disable the resume timer, if there is one.
                        if (this.resumeTimer != null)
                        {
                            Trace.WriteLine("SuspendResumePort.TryStateTransition: disabling resume timer.");
                            this.resumeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                        break;

                    case State.Closed:
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
    }
}
