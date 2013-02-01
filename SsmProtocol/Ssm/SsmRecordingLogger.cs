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
    /// <summary>
    /// Extends SsmLogger with logic to create filtered log files
    /// </summary>
    public class SsmRecordingLogger : IDisposable
    {
        /// <summary>
        /// List of parameters supported by whatever the logger is connected to.
        /// </summary>
        private ParameterDatabase database;

        /// <summary>
        /// Basic logger encapsulated by this class
        /// </summary>
        private SsmLogger logger;

        /// <summary>
        /// Profile specified by the consumer
        /// </summary>
        private LogProfile currentProfile;

        /// <summary>
        /// Profile plus log-filter parameter
        /// </summary>
        private LogProfile internalProfile;

        /// <summary>
        /// Determines what gets logged to disk
        /// </summary>
        private LogFilter filter;

        /// <summary>
        /// Delegate to invoke to create a new log writer.
        /// </summary>
        private LogWriterFactory logWriterFactory;

        private event EventHandler<LogEventArgs> logStart;
        private event EventHandler<LogEventArgs> logEntry;
        private event EventHandler<LogStopEventArgs> logStop;
        private event EventHandler<LogErrorEventArgs> logError;
        
        /// <summary>
        /// Invoked when logging begins - handler can open the CSV file and write the header
        /// </summary>
        public event EventHandler<LogEventArgs> LogStart
        {
            add { this.logStart += value; }
            remove { this.logStart -= value; }
        }

        /// <summary>
        /// Invoked when a log row is available - handler can write a line to the CSV file.
        /// </summary>
        public event EventHandler<LogEventArgs> LogEntry
        {
            add { this.logEntry += value; }
            remove { this.logEntry -= value; }
        }

        /// <summary>
        /// Invoked when logging ends - handler can close the CSV file.
        /// </summary>
        public event EventHandler<LogStopEventArgs> LogStop
        {
            add { this.logStop += value; }
            remove { this.logStop -= value; }
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
        public event EventHandler<ExceptionEventArgs> Exception
        {
            add { this.logger.Exception += value; }
            remove { this.logger.Exception -= value; }
        }

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
            get { return this.currentProfile; }
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
        private SsmRecordingLogger(
            ParameterDatabase database,
            string configurationDirectory, 
            string portName, 
            LogWriterFactory logWriterFactory)
        {
            this.logWriterFactory = logWriterFactory;
            this.database = database;
            this.filter = NeverLogFilter.GetInstance(this.logWriterFactory);
            this.logger = SsmLogger.GetInstance(configurationDirectory, portName);
            this.logger.LogStart += this.Logger_LogStart;
            this.logger.LogEntry += this.Logger_LogEntry;
            this.logger.LogStop += this.Logger_LogStop;
            this.logger.LogError += this.Logger_LogError;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~SsmRecordingLogger()
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
        public static SsmRecordingLogger GetInstance(
            ParameterDatabase database,
            string configurationDirectory, 
            string portName,
            LogWriterFactory factory)
        {
            Trace.WriteLine("SsmRecordingLogger.GetInstance");
            SsmRecordingLogger instance = new SsmRecordingLogger(
                database, 
                configurationDirectory, 
                portName, 
                factory);
            return instance;
        }

        /// <summary>
        /// Begins an asyncrhonous operation to connect to the ECU, get the 
        /// ECU ID, and load the supported parameters from the database.
        /// </summary>
        public IAsyncResult BeginConnect(AsyncCallback callback, object asyncState)
        {
            Trace.WriteLine("SsmRecordingLogger.BeginConnect");
            return this.logger.BeginConnect(callback, asyncState);
        }

        /// <summary>
        /// Complete the BeginConnect asynchronous operation
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public ParameterSource EndConnect(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmRecordingLogger.EndConnect");
            return this.logger.EndConnect(asyncResult);
        }

        /// <summary>
        /// Close serial port
        /// </summary>
        public void Disconnect()
        {
            Trace.WriteLine("SsmRecordingLogger.Disconnect");
            this.logger.Disconnect();
        }

        /// <summary>
        /// Selects parameters to be logged
        /// </summary>
        public void SetProfile(LogProfile profile)
        {
            Trace.WriteLine("SsmRecordingLogger.SetProfile");
            this.currentProfile = profile;
            if (this.currentProfile == null)
            {
                Trace.WriteLine("SsmRecordingLogger.SetProfile: profile set to null");
                return;
            }

            this.internalProfile = this.currentProfile.Clone();
            if (this.filter.Parameter != null && this.filter.Conversion != null)
            {
                this.internalProfile.Add(this.filter.Parameter, this.filter.Conversion);
            }
            this.logger.SetProfile(this.internalProfile, this.database);
        }
        
        public void SetLogFilterType(LogFilterType type)
        {
            switch (type)
            {
                case LogFilterType.NeverLog:
                    this.filter = NeverLogFilter.GetInstance(this.logWriterFactory);
                    break;

                case LogFilterType.AlwaysLog:
                    this.filter = AlwaysLogFilter.GetInstance(this.logWriterFactory);
                    break;

                case LogFilterType.Defogger:
                    this.filter = DefoggerLogFilter.GetInstance(this.logWriterFactory, this.database);
                    break;

                case LogFilterType.OpenLoop:
                    this.filter = OpenLoopLogFilter.GetInstance(this.logWriterFactory, this.database);
                    break;

                case LogFilterType.ClosedLoop:
                    this.filter = ClosedLoopLogFilter.GetInstance(this.logWriterFactory, this.database);
                    break;

                case LogFilterType.FullThrottle:
                    this.filter = FullThrottleLogFilter.GetInstance(this.logWriterFactory, this.database);
                    break;

                default:
                    throw new InvalidOperationException("Undefined log filter type: " + type.ToString());
            }

            // Update the InternalLogProfile
            this.SetProfile(this.currentProfile);
        }

        /* Not used - may not be necessary after all.
         * 
        public LogFilterType GetLogFilterType()
        {
            if (this.filter == null)
            {
                return LogFilterType.AlwaysLog;
            }

            switch (this.filter.GetType())
            {
                case typeof(NeverLogFilter);
                    return LogFilterType.NeverLog;

                case typeof (AlwaysLogFilter):
                    return LogFilterType.AlwaysLog;

                case typeof(DefoggerLogFilter):
                    return LogFilterType.Defogger;

                case typeof(OpenLoopLogFilter):
                    return LogFilterType.OpenLoop;

                case typeof(ClosedLoopLogFilter):
                    return LogFilterType.ClosedLoop;

                case (FullThrottleLogFilter):
                    return LogFilterType.FullThrottle;

            default:
                    throw new InvalidOperationException("Unexpected log filter type: " + this.filter.GetType().FullName);
            }
        }*/

        /// <summary>
        /// Starts logging
        /// </summary>
        public void StartLogging()
        {
            Trace.WriteLine("SsmRecordingLogger.StartLogging");
            this.logger.StartLogging();
        }

        /// <summary>
        /// Stops logging
        /// </summary>
        public IAsyncResult BeginStopLogging(AsyncCallback callback, object asyncState)
        {
            Trace.WriteLine("SsmRecordingLogger.BeginStopLogging");
            return this.logger.BeginStopLogging(callback, asyncState);
        }

        /// <summary>
        /// Complements BeginStopLogging
        /// </summary>
        public void EndStopLogging(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmLogger.EndStopLogging");
            this.logger.EndStopLogging(asyncResult);
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
                this.logger.Dispose();
            }
        }

        private void Logger_LogStart(object sender, LogEventArgs args)
        {
            this.filter.LogStart(args.Row);
            if (this.logStart != null)
            {
                this.logStart(this, args);
            }
        }

        private void Logger_LogEntry(object sender, LogEventArgs args)
        {
            this.filter.LogEntry(args.Row);
            if (this.logEntry != null)
            {
                this.logEntry(this, args);
            }
        }

        private void Logger_LogStop(object sender, LogStopEventArgs args)
        {
            this.filter.LogStop();
            if (this.logStop != null)
            {
                this.logStop(this, args);
            }
        }

        private void Logger_LogError(object sender, LogErrorEventArgs args)
        {
            if (this.logError != null)
            {
                this.logError(this, args);
            }
        }
    }
}
