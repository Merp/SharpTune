///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogRow.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    /// <summary>
    /// Represents a row of logging data
    /// </summary>
    [CLSCompliant(true)]
    public class LogRow
    {
        /// <summary>
        /// Columns in the row
        /// </summary>
        private ReadOnlyCollection<LogColumn> columns;

        /// <summary>
        /// Columns in the row
        /// </summary>
        public ReadOnlyCollection<LogColumn> Columns
        {
            [DebuggerStepThrough()]
            get { return this.columns; }
        }

        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        private LogRow(ReadOnlyCollection<LogColumn> columns)
        {
            this.columns = columns;
        }

        /// <summary>
        /// Factory
        /// </summary>
        internal static LogRow GetInstance(ReadOnlyCollection<LogColumn> columns)
        {
            return new LogRow(columns);
        }

        /// <summary>
        /// For debugging only - do not use in UI
        /// </summary>
        public override string ToString()
        {
            return "Columns: " + this.columns.Count;
        }

        /// <summary>
        /// Get a column by ID
        /// </summary>
        public LogColumn GetColumn(string parameterId)
        {
            foreach (LogColumn column in this.columns)
            {
                if (column.Parameter.Id == parameterId)
                {
                    return column;
                }
            }
            throw new InvalidOperationException("LogRow does not contain parameter " + parameterId);
        }
    }
}
