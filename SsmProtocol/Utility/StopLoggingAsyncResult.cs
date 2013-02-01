///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// StopLoggingAsyncResult.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    internal class StopLoggingAsyncResult : AsyncResultWithTimeout
    {
        public StopLoggingAsyncResult(
            AsyncCallback asyncCallback, 
            object asyncState) 
            :
            base(
                asyncCallback, 
                asyncState)
        {
        }
    }
}
