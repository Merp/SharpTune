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
    internal class ReadFromStreamAsyncResult : AsyncResultWithTimeout
    {
        private Stream stream;
        SsmPacketParser parser;
        
        public Stream Stream
        {
            get { return this.stream; }
        }

        public SsmPacketParser Parser
        {
            get { return this.parser; }
        }
                
        public ReadFromStreamAsyncResult(
            Stream stream, 
            SsmPacketParser parser, 
            AsyncCallback asyncCallback, 
            object asyncState) 
            :
            base(
                asyncCallback, 
                asyncState)
        {
            this.stream = stream;
            this.parser = parser;
        }
    }
}
