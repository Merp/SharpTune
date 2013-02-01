using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace NateW.Ssm
{
    /// <summary>
    /// Logs unhandled exceptions to disk.
    /// </summary>
    public class ExceptionRecorder : IDisposable
    {
        /// <summary>
        /// Writer for exception information.
        /// </summary>
        private StreamWriter writer;

        /// <summary>
        /// Serializes access to the stream writer.
        /// </summary>
        private object writerLock = new object();

        /// <summary>
        /// Number of exceptions logged
        /// </summary>
        private int exceptions;

        /// <summary>
        /// Name of exception log file
        /// </summary>
        private string fileName;

        /// <summary>
        /// Constructor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public ExceptionRecorder(string appName)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            string timestamp = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace(':', '.');
            this.fileName = Path.Combine(Path.GetTempPath(), appName + ".Shutdown." + timestamp + ".txt");
            this.writer = new StreamWriter(this.fileName);
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ExceptionRecorder()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Close the writer.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// To diagnose the tricky ones.
        /// </summary>
        /// <param name="sender">The app domain?</param>
        /// <param name="e">Contains the exception.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            lock (this.writerLock)
            {
                string message = e.ExceptionObject.ToString();
                this.writer.WriteLine(message);
                this.writer.Flush();
                this.exceptions++;

                Trace.WriteLine("Unhandled exception: " + message);
            }
        }

        /// <summary>
        /// Close the writer, and delete the file if it was not used.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Trace.WriteLine("Disposing unhandled exception log writer.");
                this.writer.Dispose();

                if (this.exceptions != 0)
                {
                    Trace.WriteLine("Retaining unhandled exception log, since there were some.");
                    return;
                }

                Trace.WriteLine("Deleting unhandled exception log, since there were none.");
                File.Delete(this.fileName);
                Trace.WriteLine("Deleted unhandled exception log.");
            }
        }
    }
}
