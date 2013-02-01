///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogFilter.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NateW.Ssm
{
    /// <summary>
    /// Creates LogWriters on demand
    /// </summary>
    /// <returns>an instance of LogWriter</returns>
    public delegate LogWriter LogWriterFactory();

    /// <summary>
    /// Writes logs only when specified conditions are met
    /// </summary>
    /// <remarks>This is the key thing for defogger-controlled logging.</remarks>
    public class LogFilter
    {
        /// <summary>
        /// For testability
        /// </summary>
        private static bool defaultBehavior = true;

        /// <summary>
        /// Creates LogWriters on demand
        /// </summary>
        private LogWriterFactory factory;

        /// <summary>
        /// Writes logs
        /// </summary>
        private LogWriter writer;

        /// <summary>
        /// The parameter required by the filter
        /// </summary>
        private Parameter parameter;

        /// <summary>
        /// The conversion required by the filter
        /// </summary>
        private Conversion conversion;

        /// <summary>
        /// 
        /// </summary>
        internal Parameter Parameter { get { return this.parameter; } }

        internal Conversion Conversion { get { return this.conversion; } }

        /// <summary>
        /// Protected constructor.  Use the factory method instead
        /// </summary>
        internal LogFilter(
            LogWriterFactory factory, 
            Parameter parameter,
            Conversion conversion)
        {
            this.factory = factory;
            this.parameter = parameter;
            this.conversion = conversion;
        }

        /// <summary>
        /// Create an instance for test use
        /// </summary>
        internal static LogFilter GetTestInstance(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion)
        {
            return new LogFilter(factory, parameter, conversion);
        }

        /// <summary>
        /// Write the names of the colums
        /// </summary>
        /// <param name="row">collection of columns, values, and conversions</param>
        public void LogStart(LogRow row)
        {
            this.ManageWriterState(row);
        }

        /// <summary>
        /// Write values from the given log entry to the stream
        /// </summary>
        /// <param name="row">collection of columns, values, and conversions</param>
        public void LogEntry(LogRow row)
        {
            if (this.ManageWriterState(row))
            {
                this.writer.LogEntry(row);
            }
        }

        /// <summary>
        /// Flush the writer
        /// </summary>
        public void LogStop()
        {
            this.SafelyDisposeWriter();
        }

        /// <summary>
        /// For testability
        /// </summary>
        internal static void SetDefaultBehavior(bool value)
        {
            LogFilter.defaultBehavior = value;
        }

        /// <summary>
        /// Base class will always log (unless testing is underway)
        /// </summary>
        protected virtual bool ShouldLog(LogColumn column)
        {
            return LogFilter.defaultBehavior;
        }

        /// <summary>
        /// Create and dispose log writer as necessary
        /// </summary>
        /// <param name="row">row of data from the SSM interface</param>
        /// <returns>true if </returns>
        private bool ManageWriterState(LogRow row)
        {
            if (this.ShouldLog(row))
            {
                if (this.writer == null)
                {
                    this.writer = this.factory();
                    this.writer.LogStart(row);
                }
                return true;
            }
            else
            {
                this.SafelyDisposeWriter();
                return false;
            }
        }

        /// <summary>
        /// Indicate whether this row should be logged 
        /// </summary>
        private bool ShouldLog(LogRow row)
        {
            // For the immutable filter types
            if (this.parameter == null)
            {
                return this.ShouldLog((LogColumn) null);
            }

            // The real logic
            foreach (LogColumn column in row.Columns)
            {
                if (column.Parameter.Id == this.parameter.Id)
                {
                    return this.ShouldLog(column);
                }
            }

            // This row doesn't contain the parameter that the log filter expects.
            // This is not unusual; the next one probably will.  
            return false;
        }

        /// <summary>
        /// Stop and dispose the writer, if there is one
        /// </summary>
        private void SafelyDisposeWriter()
        {
            if (this.writer != null)
            {
                this.writer.LogStop();
                this.writer.Dispose();
                this.writer = null;
            }
        }
    }
}
