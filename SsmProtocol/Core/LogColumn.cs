///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogColumn.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    public class DependencyMap : Dictionary<string,LogColumn>
    {
        public DependencyMap()
        {
        }
    }

    /// <summary>
    /// Represents a column in a log record
    /// </summary>
    [CLSCompliant(true)]
    public class LogColumn
    {
        /// <summary>
        /// The Parameter being logged for this column.
        /// </summary>
        private Parameter parameter;

        /// <summary>
        /// The conversion used for the Parameter.
        /// </summary>
        private Conversion conversion;

        /// <summary>
        /// Property bag.
        /// </summary>
        private PropertyBag propertyBag;

        /// <summary>
        /// The converted and formatted value for this column.
        /// </summary>
        private string columnValueAsString;

        /// <summary>
        /// The converted value for this column.
        /// </summary>
        private double columnValueAsDouble;

        /// <summary>
        /// For calculated parameters, this maps the base parameter IDs and 
        /// conversions to the columns in which they were logged
        /// </summary>
        private DependencyMap dependencyMap;

        /// <summary>
        /// If true, this column was only added because a calculated parameter depends on it
        /// </summary>
        private bool isDependency;

        /// <summary>
        /// The Parameter being logged for this column.
        /// </summary>
        public Parameter Parameter
        {
            [DebuggerStepThrough()]
            get
            {
                return this.parameter;
            }
        }

        /// <summary>
        /// The Conversion used for the parameter.
        /// </summary>
        public Conversion Conversion
        {
            [DebuggerStepThrough()]
            get
            {
                return this.conversion;
            }
        }

        /// <summary>
        /// For calculated parameters, this maps the base parameter IDs and 
        /// conversions to the columns in which they were logged
        /// </summary>
        internal DependencyMap DependencyMap
        {
            [DebuggerStepThrough()]
            get { return this.dependencyMap; }
            [DebuggerStepThrough()]
            set { this.dependencyMap = value; }
        }

        /// <summary>
        /// Indicates whether this column is calculated from other columns
        /// </summary>
        public bool IsCalculated
        {
            [DebuggerStepThrough()]
            get { return this.DependencyMap != null; }
        }

        /// <summary>
        /// Indicates whether this column was only added because a calculated parameter depends on it
        /// </summary>
        public bool IsDependency
        {
            [DebuggerStepThrough()]
            get { return this.isDependency; }
        }

        /// <summary>
        /// Property bag.
        /// </summary>
        public PropertyBag PropertyBag
        {
            get
            {
                return this.propertyBag;
            }
        }

        /// <summary>
        /// The converted and formatted value for this column.
        /// </summary>
        public string ValueAsString
        {
            [DebuggerStepThrough()]
            get { return this.columnValueAsString; }
            [DebuggerStepThrough()]
            internal set { this.columnValueAsString = value; }
        }

        /// <summary>
        /// The converted value, as a double
        /// </summary>
        public double ValueAsDouble
        {
            [DebuggerStepThrough()]
            get { return this.columnValueAsDouble; }
            [DebuggerStepThrough()]
            internal set { this.columnValueAsDouble = value; }
        }

        /// <summary>
        /// Private constructor, use factory instead.
        /// </summary>
        private LogColumn(
            Parameter parameter, 
            Conversion conversion,
            DependencyMap dependencyMap,
            bool isDependency)
        {
            this.parameter = parameter;
            this.conversion = conversion;
            this.dependencyMap = dependencyMap;
            this.isDependency = isDependency;
            this.propertyBag = new PropertyBag();
        }

        /// <summary>
        /// Factory method.
        /// </summary>
        public static LogColumn GetInstance(
            Parameter parameter,
            Conversion conversion,
            DependencyMap dependencyMap,
            bool isDependency)
        {
            return new LogColumn(
                parameter,
                conversion,
                dependencyMap,
                isDependency);
        }

        /// <summary>
        /// For debugging only, do not use in UI.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "LogColumn";

            if (this.parameter != null)
            {
                result += " Parameter: " + this.parameter.ToString();
            }

            if (this.conversion != null)
            {
                result += " Conversion: " + this.conversion.ToString();
            }

            return result;
        }
    }
}
