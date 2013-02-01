///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogWriter.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Globalization;

namespace NateW.Ssm
{
    /// <summary>
    /// Holds parameters and their conversions 
    /// </summary>
    public class LogWriter : IDisposable
    {
        /// <summary>
        /// Converts strings to ASCII and writes them to the given stream
        /// </summary>
        private TextWriter writer;

        /// <summary>
        /// If true, clock time and elapsed time will be prepended to log data
        /// </summary>
        private bool insertTimeColumns;

        /// <summary>
        /// Time the current log was started
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// Private constructor.  Use the factory method instead
        /// </summary>
        /// <param name="stream">log output stream</param>
        private LogWriter(Stream stream, bool insertTimeColumns)
        {
            this.writer = new StreamWriter(stream);
            this.insertTimeColumns = insertTimeColumns;
        }

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="stream">log output stream</param>
        /// <returns>an instance of LogWriter</returns>
        public static LogWriter GetInstance(Stream stream, bool insertTimeColumns)
        {
            return new LogWriter(stream, insertTimeColumns);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~LogWriter()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose the underlying writer
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Write the names of the colums
        /// </summary>
        /// <param name="row">collection of columns, values, and conversions</param>
        public void LogStart(LogRow row)
        {
            this.startTime = DateTime.Now;

            int extraColumns = 0;
            if (this.insertTimeColumns)
            {
                extraColumns = 2;
            }

            // TODO: Cleanup: just use a List<string>
            string[] names = new string[row.Columns.Count + extraColumns];

            if (this.insertTimeColumns)
            {
                names[0] = "Time";
                names[1] = "Clock";
            }

            int i = extraColumns;
            foreach (LogColumn column in row.Columns)
            {
                names[i] = column.Parameter.Name;
                i++;
            }

            string header = string.Join(", ", names);
            writer.WriteLine(header);
        }

        /// <summary>
        /// Write values from the given log entry to the stream
        /// </summary>
        /// <param name="row">collection of columns, values, and conversions</param>
        public void LogEntry(LogRow row)
        {
            int extraColumns = 0;
            if (this.insertTimeColumns)
            {
                extraColumns = 2;
            }

            string[] values = new string[row.Columns.Count + extraColumns];

            if (this.insertTimeColumns)
            {
                TimeSpan elapsedTime = DateTime.Now.Subtract(this.startTime);
                values[0] = ((int)elapsedTime.TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
                values[1] = DateTime.Now.ToString("yyyy-MM-dd T HH:mm:ss:fff", CultureInfo.InvariantCulture);
            }

            int i = extraColumns;
            foreach (LogColumn column in row.Columns)
            {
                values[i] = column.ValueAsString;
                i++;
            }

            string entry = string.Join(", ", values);
            writer.WriteLine(entry);
        }

        /// <summary>
        /// Flush the writer
        /// </summary>
        public void LogStop()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Dispose the writer
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.writer != null)
                {
                    this.writer.Dispose();
                    this.writer = null;
                }
            }
        }
    }

}
