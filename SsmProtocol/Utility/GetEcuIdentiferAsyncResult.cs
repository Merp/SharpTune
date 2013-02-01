///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// GetEcuIdentifierAsyncResult.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    /// <summary>
    /// AsyncResult for SsmInterface.GetEcuIdentifier
    /// </summary>
    internal class GetEcuIdentifierAsyncResult : AsyncResultWithTimeout, IDisposable
    {
        private SsmPacketParser parser;
        private byte[] readBuffer;

        public SsmPacketParser Parser
        {
            [DebuggerStepThrough()]
            get { return this.parser; }
        }
            
        public byte[] ReadBuffer
        {
            [DebuggerStepThrough()]
            get { return this.readBuffer; }
            [DebuggerStepThrough()]
            set { this.readBuffer = value; }
        }
        
        public GetEcuIdentifierAsyncResult(
            AsyncCallback asyncCallback, 
            object asyncState)
            : 
            base(
                asyncCallback, 
                asyncState)
        {
            this.parser = SsmPacketParser.CreateInstance();
        }
    }
}
