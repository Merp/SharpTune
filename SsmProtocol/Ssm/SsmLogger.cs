///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmLogger.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace NateW.Ssm
{
    public class ExceptionEventArgs : EventArgs
    {
        private Exception exception;
        public Exception Exception { get { return this.exception; } }
        public ExceptionEventArgs(Exception exception)
        {
            this.exception = exception;
        }
    }

    /// <summary>
    /// Extends SsmBasicLogger with logic to handle suspend/resume of the PC
    /// </summary>
    public class SsmLogger : IDisposable
    {
        private enum State
        {
            Unknown,
            Initial,
            Resyncing,
            WaitingForWindows,
            Opening,
            Ready,
            Closing,
            Closed,
            Disposed
        }

        /// <summary>
        /// Directory with logger.xml
        /// </summary>
        private string configurationDirectory;

        /// <summary>
        /// Name of the serial port to use for SSM.
        /// </summary>
        private string ssmPortName;

        /// <summary>
        /// Serial port
        /// </summary>
        private SerialPort port;

        /// <summary>
        /// Basic logger encapsulated by this class
        /// </summary>
        private SsmBasicLogger logger;

        /// <summary>
        /// System hangs if logging restarts too quickly after a resume from hibernation.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Indicates whether the system is initializing, ready, suspending, or suspended.
        /// </summary>
        private State state = State.Initial;

        /// <summary>
        /// For atomic state transition logging
        /// </summary>
        private object stateLock = new object();

        /// <summary>
        /// Synchronizes access to the timer
        /// </summary>
        private object timerLock = new object();

        /// <summary>
        /// Synchronizes data stream creation/destruction
        /// </summary>
        private object streamLock = new object();

        /// <summary>
        /// Delegates to invoke when there's a logging error
        /// </summary>
        private event EventHandler<LogErrorEventArgs> logError;

#if !PocketPC
        /// <summary>
        /// This is kept as a member so the handler can be unregistered during dispose
        /// </summary>
        private Microsoft.Win32.PowerModeChangedEventHandler powerModeChangedEventHandler;
#endif

        /// <summary>
        /// Invoked when logging begins - handler can open the CSV file and write the header
        /// </summary>
        public event EventHandler<LogEventArgs> LogStart
        {
            add { this.logger.LogStart += value; }
            remove { this.logger.LogStart -= value; }
        }

        /// <summary>
        /// Invoked when a log row is available - handler can write a line to the CSV file.
        /// </summary>
        public event EventHandler<LogEventArgs> LogEntry
        {
            add { this.logger.LogEntry += value; }
            remove { this.logger.LogEntry -= value; }
        }

        /// <summary>
        /// Invoked when logging ends - handler can close the CSV file.
        /// </summary>
        public event EventHandler<LogStopEventArgs> LogStop
        {
            add { this.logger.LogStop += value; }
            remove { this.logger.LogStop -= value; }
        }

        /// <summary>
        /// Invoked when an SSM error occurs.
        /// </summary>
        public event EventHandler<LogErrorEventArgs> LogError
        {
            add { this.logError += value; }
            remove { this.logError -= value; }
        }

        /// <summary>
        /// Invoked when serial-port-related errors occur.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Exception;

        /// <summary>
        /// Identifies the ECU the logger is talking to
        /// </summary>
        public string EcuIdentifier
        {
            get
            {
                return this.logger.EcuIdentifier;
            }
        }

        /// <summary>
        /// Describes the parameters that are currently being logged
        /// </summary>
        public LogProfile CurrentProfile
        {
            [DebuggerStepThrough()]
            get { return this.logger.CurrentProfile; }
        }

        /// <summary>
        /// Indicates whether the logger is logging now
        /// </summary>
        public bool IsLogging
        {
            [DebuggerStepThrough()]
            get { return this.logger.IsLogging; }
        }

        /// <summary>
        /// Addresses for the ECU query (for test use only)
        /// </summary>
        internal List<int> Addresses
        {
            [DebuggerStepThrough()]
            get { return this.logger.Addresses; }
        }

        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private SsmLogger(
            string configurationDirectory,
            string ssmPortName)
        {
            this.configurationDirectory = configurationDirectory;
            this.ssmPortName = ssmPortName;
            Stream stream = this.GetDataStream(true);
            this.logger = SsmBasicLogger.GetInstance(this.configurationDirectory, stream);
            this.TryStateTransition(State.Ready);
            this.logger.LogError += this.Logger_LogError;
#if !PocketPC
            this.powerModeChangedEventHandler = new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            Microsoft.Win32.SystemEvents.PowerModeChanged += this.powerModeChangedEventHandler;
#endif
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~SsmLogger()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Because this is IDisposable
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static SsmLogger GetInstance(
            string configurationDirectory, 
            string ssmPortName)
        {
            Trace.WriteLine("SsmLogger.GetInstance");
            SsmLogger instance = new SsmLogger(
                configurationDirectory, 
                ssmPortName);
            return instance;
        }

        /// <summary>
        /// Begins an asyncrhonous operation to connect to the ECU, get the 
        /// ECU ID, and load the supported parameters.
        /// </summary>
        public IAsyncResult BeginConnect(AsyncCallback callback, object asyncState)
        {
            Trace.WriteLine("SsmLogger.BeginConnect");
            // TODO: this is wrong...  retry logic needs to prevent IAsyncResult from being signaled until successful connect.
            IAsyncResult result = null;
            this.TryRunUnderExceptionHandler(delegate()
            {
                result = this.logger.BeginConnect(callback, asyncState);
            });
            return result;
        }

        /// <summary>
        /// Complete the BeginConnect asynchronous operation
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public ParameterSource EndConnect(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmLogger.EndConnect");
            return this.logger.EndConnect(asyncResult);
        }

        /// <summary>
        /// Close the serial port.
        /// </summary>
        public void Disconnect()
        {
            Trace.WriteLine("SsmLogger.Disconnect");
            if (this.state != State.Closed)
            {
                throw new InvalidOperationException("You must stop logging before disconnecting");
            }

            this.ReleaseSerialPort();
            this.logger.SetEcuStream(null);
        }

        /// <summary>
        /// Selects parameters to be logged
        /// </summary>
        public void SetProfile(LogProfile profile, ParameterDatabase database)
        {
            Trace.WriteLine("SsmLogger.SetProfile");
            this.logger.SetProfile(profile, database);
        }

        /// <summary>
        /// Starts logging
        /// </summary>
        public void StartLogging()
        {
            Trace.WriteLine("SsmLogger.StartLogging");
            if (this.logger.IsLogging)
            {
                Trace.WriteLine("SsmLogger.StartLogging: already logging");
                return;
            }

            Exception exception = null;
            try
            {
                this.logger.StartLogging();
            }
            catch (IOException ex)
            {
                exception = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                this.ReportException(exception);
                this.ScheduleRestart(TimeSpan.FromSeconds(1.0));
            }
        }

        /// <summary>
        /// Stops logging
        /// </summary>
        public IAsyncResult BeginStopLogging(AsyncCallback callback, object asyncState)
        {
            Trace.WriteLine("SsmLogger.BeginStopLogging");
            this.TryStateTransition(State.Closing);

            StopLoggingAsyncResult asyncResult = new StopLoggingAsyncResult(callback, asyncState);
            SsmBasicLogger localLogger = this.logger;
            if (localLogger != null)
            {
                Trace.WriteLine("SsmLogger.BeginStopLogging: stopping logger");
                return localLogger.BeginStopLogging(Logger_LoggingStopped, asyncResult);
            }
            else
            {
                Trace.WriteLine("SsmLogger.BeginStopLogging: logger gone, returning trivial AsyncResult");
                AsyncResult result = new AsyncResult(callback, asyncState);
                result.Completed(); 
                //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate { result.Completed(); }));
                return result;
            }
        }

        /// <summary>
        /// Complements BeginStopLogging
        /// </summary>
        public void EndStopLogging(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmLogger.EndStopLogging");

            SsmBasicLogger localLogger = this.logger;
            if (localLogger != null)
            {
                Trace.WriteLine("SsmLogger.EndStopLogging: invoking SsmBasicLogger.EndStopLogging()");
                localLogger.EndStopLogging(asyncResult);
            }           
        }

        /// <summary>
        /// Stop logging, in response to "Suspend" power mode event.
        /// </summary>
        /// <remarks>
        /// For use by SsmLogger and test code only.
        /// </remarks>
        internal void Suspend()
        {
            Trace.WriteLine("### PowerModeChanged: Suspend");
            this.TryStateTransition(State.Closing);
            SsmBasicLogger localLogger = this.logger;
            if (localLogger != null)
            {
                Trace.WriteLine("SsmLogger.SystemEvents_PowerModeChanged: stopping logger");
                localLogger.BeginStopLogging(Logger_LoggingStoppedForSuspend, null);
            }
        }

        /// <summary>
        /// Resume logging, in response to "Suspend" power mode event.
        /// </summary>
        /// <remarks>
        /// For use by SsmLogger and test code only.
        /// </remarks>
        internal void Resume(TimeSpan interval)
        {
            Trace.WriteLine("### PowerModeChanged: Resume");
            this.TryStateTransition(State.WaitingForWindows, delegate 
            {
                this.ScheduleRestart(interval);
            });
        }

#if !PocketPC
        /// <summary>
        /// Stop/start logging when the system hibernates and resumes
        /// </summary>
        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    this.Suspend();
                    break;

                case Microsoft.Win32.PowerModes.Resume:
                    // Must sleep a while or the serial port won't open properly
                    this.Resume(TimeSpan.FromSeconds(10));
                    break;
            }
        }
#endif

        /// <summary>
        /// Invoked after the logger has stopped, keeps the port open.
        /// </summary>
        private void Logger_LoggingStopped(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmLogger.Logger_LoggingStopped");
            this.TryStateTransition(State.Closed);

            AsyncResult nested = asyncResult.AsyncState as AsyncResult;
            if (nested != null)
            {
                nested.Completed();
            }
        }

        /// <summary>
        /// Invoked after the logger has stopped, closes the port.
        /// </summary>
        private void Logger_LoggingStoppedForSuspend(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmLogger.Logger_LoggingStopped: Releasing serial port.");
            this.ReleaseSerialPort();
            this.TryStateTransition(State.Closed);

            AsyncResult nested = asyncResult.AsyncState as AsyncResult;
            if (nested != null)
            {
                nested.Completed();
            }
        }

        /// <summary>
        /// Hook LogError events and schedule a restart
        /// </summary>
        private void Logger_LogError(object sender, LogErrorEventArgs args)
        {
            Trace.WriteLine("SsmLogger.Logger_LogError: " + SsmUtility.GetExceptionMessage(args.Exception));

            if (this.logError != null)
            {
                this.logError(sender, args);
            }

            this.TryStateTransition(State.Resyncing, delegate
            {
                this.ScheduleRestart(TimeSpan.FromSeconds(1.0));
            });
        }

        /// <summary>
        /// Schedule a restart of the logger
        /// </summary>
        private void ScheduleRestart(TimeSpan interval)
        {
            Trace.WriteLine("SsmLogger.ScheduleRestart invoked: " + interval.ToString());

            lock (this.timerLock)
            {
                if (this.timer != null)
                {
                    Trace.WriteLine("SsmLogger.ScheduleRestart: bailing out, we're already waiting for a timer tick.");
                    return;
                }

                this.timer = new Timer(
                    this.TimerCallback,
                    null, 
                    interval, 
                    TimeSpan.FromMilliseconds(-1));
            }
        }

        /// <summary>
        /// Restart logging after a system resume
        /// </summary>
        private void TimerCallback(object unused)
        {
            Trace.WriteLine("SsmLogger.TimerCallback: State is " + this.state.ToString());
            lock (this.timerLock)
            {
                if (this.state == State.Disposed)
                {
                    Trace.WriteLine("SsmLogger.TimerCallback: bailing out, we've been disposed");
                    return;
                }

                this.timer = null;
                if (this.state != State.WaitingForWindows && this.state != State.Resyncing)
                {
                    Trace.WriteLine("SsmLogger.TimerCallback: current state is " + this.state);
                    return;
                }

                if (!this.TryStateTransition(State.Opening, ReopenStream))
                {
                    Trace.WriteLine("SsmLogger.TimerCallback: aborting.");
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReopenStream()
        {
            if ((this.logger != null) && this.logger.IsLogging)
            {
                Trace.WriteLine("SsmLogger.TimerCallback: logger is already running.");
                return;
            }

            Trace.WriteLine("SsmLogger.TimerCallback: Attempting restart.");
            Stream stream = this.GetDataStream(false);
            if (stream == null)
            {
                Trace.WriteLine("SsmLogger.ReopenStream: Unable to open stream, will try again in one second.");

                if (!this.TryStateTransition(State.Resyncing))
                {
                    Trace.WriteLine("SsmLogger.ReopenStream: Going to reset the timer anyway...");
                }

                this.ScheduleRestart(TimeSpan.FromSeconds(1));
                return;
            }

            Trace.WriteLine("SsmLogger.ReopenStream: Stream opened.");
            this.logger.SetEcuStream(stream);
            this.StartLogging();

            if (!this.TryStateTransition(State.Ready))
            {
                Trace.WriteLine("SsmLogger.ReopenStream: Transition to Ready was illegal, stopping.");
                SsmBasicLogger localLogger = this.logger;
                if (localLogger != null)
                {
                    localLogger.BeginStopLogging(Logger_LoggingStopped, null);
                }
            }
        }

        /// <summary>
        /// Get the stream from which to read ECU data
        /// </summary>
        private Stream GetDataStream(bool throwOnFailure)
        {
            Trace.WriteLine("SsmLogger.GetDataStream, throwOnFailure = " +
                throwOnFailure.ToString(CultureInfo.InvariantCulture) +
                " Thread = " +
                Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));

            Stream result = null;
            lock (this.streamLock)
            {
                if (this.state != State.Disposed)
                {
                    result = GetDataStream_Synchronized(throwOnFailure);
                }
                else
                {
                    Trace.WriteLine("SsmLogger.GetDataStream: we've been disposed, skipping.");
                }
            }
            return result;
        }

        private Stream GetDataStream_Synchronized(bool throwOnFailure)
        {
            Trace.WriteLine("SsmLogger.GetDataStream_Synchronized, ssmPortName = " +
                this.ssmPortName +
                ", throwOnFailure = " +
                throwOnFailure.ToString(CultureInfo.InvariantCulture) +
                ", Thread = " +
                Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));

            if (throwOnFailure)
            {
                return SsmUtility.GetDataStream(
                    this.ssmPortName, 
                    4800, 
                    ref this.port,
                    delegate(string line)
                    {
                        Trace.WriteLine(line);
                    });
            }
            else
            {
                Exception exception = null;
                try
                {
                    return SsmUtility.GetDataStream(
                        this.ssmPortName,
                        4800,
                        ref this.port,
                        delegate(string line)
                        {
                            Trace.WriteLine(line);
                        });
                }
                catch (System.Security.SecurityException ex)
                {
                    exception = ex;
                }
                catch (System.TimeoutException ex)
                {
                    exception = ex;
                }
                catch (System.IO.IOException ex)
                {
                    exception = ex;
                }
                catch (System.UnauthorizedAccessException ex)
                {
                    exception = ex;
                }

                if (throwOnFailure)
                {
                    throw exception;
                }

                this.ReportException(exception);
                Thread.Sleep(1000);
                return null;
            }
        }

        /// <summary>
        /// Close port, release stream
        /// </summary>
        private void ReleaseSerialPort()
        {
            Trace.WriteLine("SsmLogger.ReleaseSerialPort, Thread = " +
                Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));

            lock (this.streamLock)
            {
                Trace.WriteLine("SsmLogger.ReleaseSerialPort, Thread = " +
                    Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture) +
                    ", inside lock");

                if (this.port != null)
                {
                    Trace.WriteLine("SsmLogger.ReleaseSerialPort: closing port");
                    this.port.Close();

                    Trace.WriteLine("SsmLogger.ReleaseSerialPort: disposing port");
                    this.port.Dispose();
                    this.port = null;
                }
                else
                {
                    Trace.WriteLine("SsmLogger.ReleaseSerialPort: there is no port.");
                }
            }

            Trace.WriteLine("SsmLogger.ReleaseSerialPort: returning");
        }

        /// <summary>
        /// Report an exception
        /// </summary>
        private void ReportException(Exception exception)
        {
            if(SsmUtility.IsTransientException(exception))
            {
                Trace.WriteLine("SsmLogger.ReportException: " + exception.Message);
            }
            else
            {
                Trace.WriteLine("SsmLogger.ReportException: " + exception.ToString());
            }

            EventHandler<ExceptionEventArgs> handler = this.Exception;
            if (handler != null)
            {
                handler(this, new ExceptionEventArgs(exception));
            }
        }

        /// <summary>
        /// Switch states, and log the transition
        /// </summary>
        private bool TryStateTransition(State newState)
        {
            return this.TryStateTransition(newState, null);
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
                    "SsmLogger.StateTransition: from {0} to {1} on thread {2}",
                    this.state,
                    newState,
                    Thread.CurrentThread.ManagedThreadId);
                Trace.WriteLine(message);

                switch (newState)
                {
                    case State.Initial:
                        throw new InvalidOperationException("Should never transition to state Initial.");

                    case State.Resyncing:
                    case State.Opening:
                    case State.Ready:
                        if (this.state == State.Closing ||
                            this.state == State.Closed || 
                            this.state == State.Disposed)
                        {
                            Trace.WriteLine("SsmLogger.TryStateTransition: illegal transition.");
                            return false;
                        }
                        break;

                    case State.WaitingForWindows:
                        if (this.state != State.Closed)
                        {
                            Trace.WriteLine("SsmLogger.TryStateTransition: illegal transition.");
                            return false;
                        }
                        break;
                    
                    case State.Closing:
                        break;

                    case State.Closed:
                        if (this.state != State.Closing)
                        {
                            Trace.WriteLine(string.Format(
                                CultureInfo.InvariantCulture,
                                "SsmLogger.TryStateTransition: suspicious transition: from {0} to Closed",
                                this.state));
                        }
                        break;

                    case State.Disposed:
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
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
#if !PocketPC
                Microsoft.Win32.SystemEvents.PowerModeChanged -= this.powerModeChangedEventHandler;
#endif

                this.TryStateTransition(State.Disposed);

                lock (this.streamLock)
                {
                    if (this.port != null)
                    {
                        this.port.Dispose();
                        this.port = null;
                    }
                }

                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }

        /// <summary>
        /// Consider moving this to SsmBasicLogger, and report exceptions via LogError
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool TryRunUnderExceptionHandler(VoidVoid code)
        {
            Exception exception = null;

            try
            {
                code();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            this.ReportException(exception);
            return false;
        }
    }
}
