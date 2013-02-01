///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// ReadFromStreamAsyncResult.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    internal class AsyncResultWithTimeout : AsyncResult, IDisposable
    {
        private Timer timer;

        public AsyncResultWithTimeout(
            AsyncCallback asyncCallback,
            object asyncState)
            :
            base(asyncCallback, asyncState)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            {
                TimerCallback timeoutCallback = new TimerCallback(this.TimeoutExpired);
                this.timer = new Timer(
                    new System.Threading.TimerCallback(timeoutCallback),
                    null,
                    1000,
                    System.Threading.Timeout.Infinite);
            }            
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AsyncResultWithTimeout()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }
        
        /// <summary>
        /// Throws a TimeoutException via the IAsyncResult if the operation is not yet complete.
        /// </summary>
        private void TimeoutExpired(object state)
        {
            if (!this.TrySetCompletionFlag())
            {
                return;
            }

            this.Exception = new SsmPacketFormatException("Timeout expired, packet incomplete.");
            this.InternalCompleted();
        }
    }
}
