///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// MultipleReadAsyncResult.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    /// <summary>
    /// AsyncResult for SsmInterface.BeginBlockRead
    /// </summary>
    internal class BlockReadAsyncResult : AsyncResult
    {
        private SsmPacketParser parser;
        private List<byte> values;

        internal SsmPacketParser Parser
        {
            get { return this.parser; }
            set { this.parser = value; }
        }

        public byte[] Values
        {
            // TODO: optimize
            get { return this.values.ToArray(); }
        }

        internal IList<byte> ValueList
        {
            get { return this.values; }
        }

        public BlockReadAsyncResult(
            AsyncCallback asyncCallback, 
            object asyncState)
            :
            base (
                asyncCallback,
                asyncState)
        {
            this.parser = SsmPacketParser.CreateInstance();
            this.values = new List<byte>();
        }
    }
}
