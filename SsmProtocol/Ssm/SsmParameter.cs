///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmParameter.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace NateW.Ssm
{
    /// <summary>
    /// Describes a parameter in the SSM database
    /// </summary>
    [CLSCompliant(true)]
    public class SsmParameter : Parameter
    {
        /// <summary>
        /// Address to read
        /// </summary>
        private int address;

        /// <summary>
        /// Number of bytes to read
        /// </summary>
        private int length;

        /// <summary>
        /// Byte in the ECU capability vector that indicates whether this parameter is supported
        /// </summary>
        private int ecuCapabilityByteIndex;

        /// <summary>
        /// Bit in the ECU capability vector that indicates whether this parameter is supported
        /// </summary>
        private int ecuCapabilityBitIndex;

        /// <summary>
        /// Address to read
        /// </summary>
        public int Address
        {
            [DebuggerStepThrough()]
            get 
            {
                if (this.Dependencies != null)
                {
                    throw new InvalidOperationException("This is a calculated parameter, it has no address.");
                }
                return this.address;
            }

            [DebuggerStepThrough()]
            internal set { this.address = value; }
        }

        /// <summary>
        /// Number of bytes to read
        /// </summary>
        public int Length 
        {
            [DebuggerStepThrough()]
            get { return this.length; }
        }

        /// <summary>
        /// Byte in the ECU capability vector that indicates whether this parameter is supported
        /// </summary>
        public int EcuCapabilityByteIndex
        {
            [DebuggerStepThrough()]
            get { return this.ecuCapabilityByteIndex; }
        }

        /// <summary>
        /// Bit in the ECU capability vector that indicates whether this parameter is supported
        /// </summary>
        public int EcuCapabilityBitIndex 
        {
            [DebuggerStepThrough()]
            get { return this.ecuCapabilityBitIndex; }
        }

        /// <summary>
        /// Constructor for extended parameters
        /// </summary>
        public SsmParameter(
            ParameterSource source,
            string id,
            string name,
            int address,
            int length,
            IList<Conversion> conversions)
            :
            this(
                source,
                id,
                name,
                address,
                length,
                conversions,
                0,
                0,
                null)
        {
        }
                
        /// <summary>
        /// Constructor for basic parameters
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte")]
        public SsmParameter(
            ParameterSource source,
            string id,
            string name,
            int address,
            int length,
            IList<Conversion> conversions,
            int ecuCapabilityByteIndex,
            int ecuCapabilityBitIndex,
            IList<Parameter> dependencies)
            :
            base(
            source,
            id,
            name,
            conversions,
            dependencies)
        {
            this.address = address;
            this.length = length;
            this.ecuCapabilityByteIndex = ecuCapabilityByteIndex;
            this.ecuCapabilityBitIndex = ecuCapabilityBitIndex;
        }
    }
}