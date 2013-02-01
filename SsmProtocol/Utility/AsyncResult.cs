///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// AsyncResult.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    /// <summary>
    /// Base class for IAsyncResult implementation
    /// </summary>
    internal class AsyncResult : IAsyncResult
    {
        private object asyncState;
        private ManualResetEvent waitHandle;
        private Exception exception;
        private int completions;
        private AsyncCallback asyncCallback;
#if !PocketPC
        private StackTrace stackTrace;
#endif        
        /// <summary>
        /// Exception to be thrown from .EndWhatever()
        /// </summary>
        public Exception Exception
        {
            [DebuggerStepThrough()]
            set { this.exception = value; }

            [DebuggerStepThrough()]
            get { return this.exception; }
        }

        /// <summary>
        /// Shows where this AsyncResult was created.
        /// </summary>
        public string StackTraceString
        {
            get { return this.stackTrace.ToString(); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncResult(AsyncCallback asyncCallback, object asyncState)
        {
            this.asyncCallback = asyncCallback;
            this.asyncState = asyncState;
            this.waitHandle = new ManualResetEvent(false);

#if !PocketPC
            if (Debugger.IsAttached)
            {
                this.stackTrace = new StackTrace();
            }
#endif
        }

        /// <summary>
        /// Invoke this when the async operation is completed
        /// </summary>
        public void Completed()
        {
            if (!TrySetCompletionFlag())
            {
                return;
            }
            this.InternalCompleted();
        }

        /// <summary>
        /// Indicates type, and (if debugger was attached) where created
        /// </summary>
        public override string ToString()
        {
            string result = this.GetType().Name;

#if !PocketPC
            if (this.stackTrace != null)
            {
                result += ", created at " + this.stackTrace.ToString();
            }
#endif
            if (this.asyncState != null)
            {
                result += ", asyncState: " + this.asyncState.ToString();
            }

            return result;
        }

        #region IAsyncResult Members

        public object AsyncState
        {
            [DebuggerStepThrough()]
            get { return this.asyncState; }
        }

        public WaitHandle AsyncWaitHandle
        {
            [DebuggerStepThrough()]
            get { return this.waitHandle; }
        }

        public bool CompletedSynchronously
        {
            [DebuggerStepThrough()]
            get { return false; }
        }

        public bool IsCompleted
        {
            [DebuggerStepThrough()]
            get { return this.completions != 0; }
        }

        #endregion

        internal bool TrySetCompletionFlag()
        {
            // First one wins, second and later are ignored
            if (Interlocked.CompareExchange(ref this.completions, 1, 0) == 1)
            {
                return false;
            }
            return true;
        }

        internal void InternalCompleted()
        {
            if (this.asyncCallback != null)
            {
                this.asyncCallback(this);
            }

            ((ManualResetEvent)this.AsyncWaitHandle).Set();
        }
    }
}
