///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SerializedColumn.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NateW.Ssm
{
    /// <summary>
    /// Used to serialize a column in a log profile
    /// </summary>
    public class SerializedColumn
    {
        private string parameterId;
        private string parameterName;
        private string conversionUnits;
        
        public string ParameterId
        {
            [DebuggerStepThrough()]
            get { return this.parameterId; }
            [DebuggerStepThrough()]
            set { this.parameterId = value; }
        }

        public string ParameterName
        {
            [DebuggerStepThrough()]
            get { return this.parameterName; }
            [DebuggerStepThrough()]
            set { this.parameterName = value; }
        }

        public string ConversionUnits
        {
            [DebuggerStepThrough()]
            get { return this.conversionUnits; }
            [DebuggerStepThrough()]
            set { this.conversionUnits = value; }
        }
                
        /// <summary>
        /// Required for XML serialization
        /// </summary>
        public SerializedColumn()
        {
        }

        /// <summary>
        /// More convenient than the above
        /// </summary>
        public SerializedColumn(
            string parameterId,
            string parameterName,
            string conversionUnits)
        {
            this.parameterId = parameterId;
            this.parameterName = parameterName;
            this.conversionUnits = conversionUnits;
        }

        /// <summary>
        /// For debugger only - do not use in UI
        /// </summary>
        public override string ToString()
        {
            return this.parameterName + " (" + this.parameterId + ") " + this.conversionUnits;
        }
    }
}
