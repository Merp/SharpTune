///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmLogger.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace NateW.Ssm
{
    /// <summary>
    /// EventArgs sent when starting a log and when each log row is received
    /// </summary>
    public class LogEventArgs : System.EventArgs
    {
        private LogRow row;
        private object userData;

        public LogRow Row
        {
            [DebuggerStepThrough()]
            get { return this.row; }
        }

        public object UserData
        {
            [DebuggerStepThrough()]
            get { return this.userData; }
        }

        internal LogEventArgs(LogRow row, object userData)
        {
            this.row = row;
            this.userData = userData;
        }
    }

    /// <summary>
    /// EventArgs sent when logging stops
    /// </summary>
    public class LogStopEventArgs : System.EventArgs
    {
        private object userData;

        public object UserData
        {
            [DebuggerStepThrough()]
            get { return this.userData; }
        }

        internal LogStopEventArgs(object userData)
        {
            this.userData = userData;
        }
    }

    /// <summary>
    /// EventArgs sent when an error interrupts logging
    /// </summary>
    public class LogErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Exception
        /// </summary>
        private Exception exception;

        /// <summary>
        /// The Exception thrown by the logging operation
        /// </summary>
        public Exception Exception
        {
            [DebuggerStepThrough()]
            get { return this.exception; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal LogErrorEventArgs(Exception exception)
        {
            this.exception = exception;
        }
    }

    /// <summary>
    /// AsyncResult for the SsmLogger.Connect operation
    /// </summary>
    internal class ConnectAsyncResult : AsyncResult
    {
        public ParameterSource ParameterSource { get; set; }

        public ConnectAsyncResult(AsyncCallback asyncCallback, object asyncState) :
            base (asyncCallback, asyncState)
        {
        }
    }

    /// <summary>
    /// Application-facing API for logging with SSM
    /// </summary>
    /// <remarks>
    /// Encapsulates the SSM protocol and parameter database,
    /// and takes care of logging start/stop logic.
    /// </remarks>
    public class SsmBasicLogger
    {
        /// <summary>
        /// Logger state
        /// </summary>
        private enum LoggerState
        {
            Stopped,
            Starting,
            Logging,
            Stopping
        }

        /// <summary>
        /// Filter string for Open and Save file dialogs
        /// </summary>
        public static readonly string FileDialogFilterString = string.Format(
            CultureInfo.InvariantCulture,
            "Profiles (*{0})|*{0}|All Files (*.*)|*.*", 
            SsmBasicLogger.DefaultProfileExtension, 
            SsmBasicLogger.DefaultProfileExtension);

        /// <summary>
        /// Default extension to use for logger profile files
        /// </summary>
        public const string DefaultProfileExtension = ".profile";

        /// <summary>
        /// Identifies the ECU the logger is talking to
        /// </summary>
        private SsmInterface ecu;

        /// <summary>
        /// Directory with logger.xml
        /// </summary>
        private string configurationDirectory;

        /// <summary>
        /// Holds the public LogProfile and the associated event args, addresses, raw data, etc
        /// </summary>
        private InternalLogProfile internalProfile;

        /// <summary>
        /// If true, logging is active
        /// </summary>
        private LoggerState state = LoggerState.Stopped;

        /// <summary>
        /// Stopping must be done asynchronously
        /// </summary>
        private AsyncResult stopLoggingAsyncResult;

        /// <summary>
        /// For atomic state transition logging
        /// </summary>
        private object stateLock = new object();

        /// <summary>
        /// Invoked when logging begins - handler can open the CSV file and write the header
        /// </summary>
        public event EventHandler<LogEventArgs> LogStart;

        /// <summary>
        /// Invoked when a log row is available - handler can write a line to the CSV file
        /// </summary>
        public event EventHandler<LogEventArgs> LogEntry;

        /// <summary>
        /// Invoked when logging ends - handler can close the CSV file
        /// </summary>
        public event EventHandler<LogStopEventArgs> LogStop;

        /// <summary>
        /// Invoked when an error occures - handler may be able to recover by flushing the serial port's IO buffers
        /// </summary>
        public event EventHandler<LogErrorEventArgs> LogError;

        /// <summary>
        /// Identifies the ECU the logger is talking to
        /// </summary>
        public string EcuIdentifier
        {
            get
            {
                if (this.ecu == null)
                {
                    throw new InvalidOperationException("Connect first.");
                }
                return this.ecu.EcuIdentifier;
            }
        }

        /// <summary>
        /// Describes the parameters that are currently being logged
        /// </summary>
        public LogProfile CurrentProfile
        {
            [DebuggerStepThrough()]
            get { return this.internalProfile.LogProfile; }
        }

        /// <summary>
        /// Indicates whether the logger is logging now
        /// </summary>
        public bool IsLogging
        {
            [DebuggerStepThrough()]
            get { return this.state != LoggerState.Stopped; }
        }

        /// <summary>
        /// Addresses for the ECU query (for test use only)
        /// </summary>
        internal List<int> Addresses
        {
            [DebuggerStepThrough()]
            get { return this.internalProfile.Addresses; }
        }

        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        private SsmBasicLogger(string configurationDirectory, Stream stream)
        {
            this.configurationDirectory = configurationDirectory;
            this.ecu = SsmInterface.GetInstance(stream);            
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static SsmBasicLogger GetInstance(
            string configurationDirectory, 
            Stream stream)
        {
            Trace.WriteLine("SsmBasicLogger.GetInstance");
            SsmBasicLogger instance = new SsmBasicLogger(configurationDirectory, stream);
            return instance;
        }

        /// <summary>
        /// Conduct all future ECU transactions over the given stream
        /// </summary>
        public void SetEcuStream(Stream stream)
        {
            Trace.WriteLine("SsmBasicLogger.SetEcuStream");
            this.ecu = SsmInterface.GetInstance(stream);
        }

        /// <summary>
        /// Begins an asyncrhonous operation to connect to the ECU, get the 
        /// ECU ID, and load the supported parameters from the database.
        /// </summary>
        public IAsyncResult BeginConnect(AsyncCallback callback, object asyncState)
        {
            Trace.WriteLine("SsmBasicLogger.BeginConnect");
            ConnectAsyncResult asyncResult = new ConnectAsyncResult(callback, asyncState);
            this.ecu.BeginGetEcuIdentifier(GetEcuIdentifierCallback, asyncResult);
            return asyncResult;
        }

        /// <summary>
        /// Complete the BeginConnect asynchronous operation
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public ParameterSource EndConnect(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmBasicLogger.EndConnect");
            ConnectAsyncResult internalState = (ConnectAsyncResult)asyncResult;
            if (internalState.Exception != null)
            {
                throw internalState.Exception;
            }

            return internalState.ParameterSource;
        }

        /// <summary>
        /// Selects parameters to be logged
        /// </summary>
        public void SetProfile(LogProfile profile, ParameterDatabase database)
        {
            Trace.WriteLine("SsmBasicLogger.SetProfile");
            this.internalProfile = InternalLogProfile.GetInstance(profile, database);
        }

        /// <summary>
        /// Starts logging
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public void StartLogging()
        {
            Trace.WriteLine("SsmBasicLogger.StartLogging");
            if (this.IsLogging)
            {
                throw new InvalidOperationException("Logging is already active.");
            }

            if (this.internalProfile == null)
            {
                throw new InvalidOperationException("Call SetProfile() before StartLogging()");
            }

/*            if (this.internalProfile.Addresses.Count == 0)
            {
                throw new InvalidOperationException("Logging profile is empty.");
            }
*/
            this.SetState(LoggerState.Starting, "StartLogging");

            try
            {
                if (this.LogStart != null)
                {
                    this.LogStart(this, this.internalProfile.LogEventArgs);
                }
            }
            catch (IOException exception)
            {
                Trace.WriteLine("SsmBasicLogger.StartLogging: error in call to LogStart:");
                Trace.WriteLine(exception.ToString());
            }

            this.SetState(LoggerState.Logging, "StartLogging");
            this.ecu.BeginMultipleRead(this.internalProfile.Addresses, EndMultipleReadCallback, this.internalProfile);
        }

        /// <summary>
        /// Stops logging
        /// </summary>
        /// <remarks>
        /// This sets the state to Stopping, but since this runs on the main thread,
        /// and the EndMultipleRead callback needs to use the main thread to update
        /// the UI, EndMultipleRead will be blocked until this returns.  This means
        /// that the state cannot transition from Stopping to Stopped during this 
        /// method.  Instead it should return an IAsyncResult which completes when
        /// the state transitions to Stopped.
        /// </remarks>
        public IAsyncResult BeginStopLogging(AsyncCallback callback, object asyncState)
        {
            Trace.WriteLine("SsmBasicLogger.BeginStopLogging");
            if (this.state == LoggerState.Stopped)
            {
                Trace.WriteLine("SsmBasicLogger.BeginStopLogging: Already stopped.");
                AsyncResult asyncResult = new AsyncResult(callback, asyncState);
                asyncResult.Completed();
                return asyncResult;
            }
            else
            {
                Trace.WriteLine("SsmBasicLogger.BeginStopLogging: Changing state to Stopping.");
                this.SetState(LoggerState.Stopping, "BeginStopLogging", delegate()
                {
                    this.stopLoggingAsyncResult = new AsyncResult(callback, asyncState);
                });
                return this.stopLoggingAsyncResult;
            }
        }

        /// <summary>
        /// Complements BeginStopLogging
        /// </summary>
        public void EndStopLogging(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmBasicLogger.EndStopLogging");
            // No-op, just here to complete the async pattern

            // Make StyleCop happy:
            asyncResult.ToString();
            this.ToString();
        }
                
        /// <summary>
        /// For test use only
        /// </summary>
        internal LogEventArgs GetOneRow()
        {
            byte[] rawValues = this.ecu.SyncReadMultiple(this.internalProfile.Addresses);
            this.internalProfile.StoreSsmValues(rawValues);
            return this.internalProfile.LogEventArgs;
        }
                
        /// <summary>
        /// Invoked by the SsmInterface when the ECU identifier has been received
        /// </summary>
        private void GetEcuIdentifierCallback(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmBasicLogger.GetEcuIdentifierCallback");
            ConnectAsyncResult internalState = (ConnectAsyncResult) asyncResult.AsyncState;

            try
            {
                this.ecu.EndGetEcuIdentifier(asyncResult);

                Trace.WriteLine("SsmBasicLogger.GetEcuIdentifierCallback: creating database");

                // TODO: remove ParameterDatabase from SsmBasicLogger, pass ParameterSource to GetEcuIdentifier callback
                internalState.ParameterSource = SsmParameterSource.GetInstance(
                    configurationDirectory, 
                    this.ecu.EcuIdentifier, 
                    this.ecu.CompatibilityMap);
            }
            catch (UnauthorizedAccessException ex)
            {
                internalState.Exception = ex;
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
            }
            catch (System.Security.SecurityException ex)
            {
                internalState.Exception = ex;
            }
            internalState.Completed();
        }

        /// <summary>
        /// Log state changes
        /// </summary>
        private void SetState(LoggerState newState, string message)
        {
            this.SetState(newState, message, null);
        }

        /// <summary>
        /// Log state changes
        /// </summary>
        private void SetState(LoggerState newState, string message, VoidVoid atomicOperation)
        {
            lock (this.stateLock)
            {
                Trace.WriteLine("SsmBasicLogger.SetState: invoked by " + message);
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "SsmBasicLogger.SetState: from {0} to {1}", this.state, newState));
                this.state = newState;

                if (atomicOperation != null)
                {
                    atomicOperation();
                }
            }
        }

        /// <summary>
        /// Invoked by the SsmInterface when the read response has been received
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void EndMultipleReadCallback(IAsyncResult asyncResult)
        {
            byte[] rawValues;
            Exception exception = null;
            InternalLogProfile oldProfile = (InternalLogProfile)asyncResult.AsyncState;
                
            try
            {
                rawValues = this.ecu.EndMultipleRead(asyncResult);
                oldProfile.StoreSsmValues(rawValues);
                if (this.LogEntry != null)
                {
                    this.LogEntry(this, oldProfile.LogEventArgs);
                }

                if (this.state == LoggerState.Logging)
                {
                    InternalLogProfile newProfile = this.internalProfile;
                    if (newProfile != oldProfile)
                    {
                        if (this.LogStop != null)
                        {
                            LogStopEventArgs stopArgs = new LogStopEventArgs(oldProfile.LogEventArgs.UserData);
                            this.LogStop(this, stopArgs);
                        }

                        if (this.LogStart != null)
                        {
                            this.LogStart(this, newProfile.LogEventArgs);
                        }
                    }

                    this.ecu.BeginMultipleRead(newProfile.Addresses, EndMultipleReadCallback, newProfile);
                }
                else
                {
                    this.SetState(LoggerState.Stopped, "EndMultipleReadCallback", delegate
                    {
                        if (this.stopLoggingAsyncResult != null)
                        {
                            this.stopLoggingAsyncResult.Completed();
                            this.stopLoggingAsyncResult = null;
                        }
                    });

                    if (this.LogStop != null)
                    {
                        LogStopEventArgs stopArgs = new LogStopEventArgs(oldProfile.LogEventArgs.UserData);
                        this.LogStop(this, stopArgs);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                exception = ex;
            }
            catch (IOException ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                Trace.WriteLine("SsmBasicLogger.EndMultipleReadCallback: Exception thrown from consumer's log event handler");
                LogErrorEventArgs args = new LogErrorEventArgs(exception);
                if (this.LogError != null)
                {
                    this.LogError(this, args);
                }

                this.SetState(LoggerState.Stopped, "EndMultipleReadCallback, exception thrown");
                if (this.LogStop != null)
                {
                    LogStopEventArgs stopArgs = new LogStopEventArgs(oldProfile.LogEventArgs.UserData);
                    this.LogStop(this, stopArgs);
                }

                if (this.stopLoggingAsyncResult != null)
                {
                    this.stopLoggingAsyncResult.Completed();
                    this.stopLoggingAsyncResult = null;
                }
            }
        }
    }
}
