///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogFilters.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NateW.Ssm
{
    /// <summary>
    /// Indicates what condition must be met to record rows to disk.
    /// </summary>
    public enum LogFilterType
    {
        Invalid = 0,
        NeverLog,
        AlwaysLog,
        Defogger,
        OpenLoop,
        ClosedLoop,
        FullThrottle,
    }

    /// <summary>
    /// For testing purposes
    /// </summary>
    internal class TestLogFilter : LogFilter
    {
        private string requiredValue;

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="factory">Creates LogWriters on-demand</param>
        /// <returns>an instance of LogFilter</returns>
        public static LogFilter GetInstance(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion,
            string requiredValue)
        {
            return new TestLogFilter(factory, parameter, conversion, requiredValue);
        }

        private TestLogFilter(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion,
            string requiredValue)
            : base (
            factory,
            parameter,
            conversion)
        {
            this.requiredValue = requiredValue;
        }

        protected override bool ShouldLog(LogColumn column)
        {
            return column.ValueAsString == requiredValue;
        }
    }

    /// <summary>
    /// Supports AlwaysLogFilter and NeverLogFilter
    /// </summary>
    public class TrivialLogFilter : LogFilter
    {
        private bool state;

        internal TrivialLogFilter(
            LogWriterFactory factory,
            bool state)
            : base (
            factory,
            null,
            null)
        {
            this.state = state;
        }

        protected override bool ShouldLog(LogColumn column)
        {
            return state;
        }
    }

    /// <summary>
    /// Always writes to the log
    /// </summary>
    public static class AlwaysLogFilter
    {
        public static LogFilter GetInstance(LogWriterFactory factory)
        {
            return new TrivialLogFilter(factory, true);
        }
    }

    /// <summary>
    /// Never writes to the log
    /// </summary>
    public static class NeverLogFilter
    {
        public static LogFilter GetInstance(LogWriterFactory factory)
        {
            return new TrivialLogFilter(factory, false);
        }
    }

    /// <summary>
    /// Defogger-controlled logging
    /// </summary>
    public class DefoggerLogFilter : LogFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private DefoggerLogFilter(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion)
            : base(
            factory,
            parameter,
            conversion)
        {
        }
            
        /// <summary>
        /// Factory
        /// </summary>
        public static LogFilter GetInstance(
            LogWriterFactory factory,
            ParameterDatabase database)
        {
            Parameter parameter;
            Conversion conversion;
            if (database.TryGetParameterById("S20", out parameter) && 
                parameter.TryGetConversionByUnits("boolean", out conversion))
            {
                return new DefoggerLogFilter(factory, parameter, conversion);
            }
            else
            {
                return new LogFilter(factory, null, null);
            }
        }

        /// <summary>
        /// Log when defogger is on
        /// </summary>
        protected override bool ShouldLog(LogColumn column)
        {
            return string.Compare(column.ValueAsString, Conversion.BooleanTrue, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    /// <summary>
    /// Open loop logging
    /// </summary>
    public class OpenLoopLogFilter : LogFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private OpenLoopLogFilter(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion)
            : base(
            factory,
            parameter,
            conversion)
        {
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static LogFilter GetInstance(
            LogWriterFactory factory,
            ParameterDatabase database)
        {
            Parameter parameter;
            Conversion conversion;
            if ((database.TryGetParameterById("E3", out parameter) ||
                database.TryGetParameterById("E33", out parameter)) &&
                parameter.TryGetConversionByUnits("status", out conversion))
            {
                return new OpenLoopLogFilter(factory, parameter, conversion);
            }
            else
            {
                return new LogFilter(factory, null, null);
            }
        }

        /// <summary>
        /// Log when in open loop
        /// </summary>
        protected override bool ShouldLog(LogColumn column)
        {
            return string.Compare(column.ValueAsString, "10", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    /// <summary>
    /// Closed loop logging
    /// </summary>
    public class ClosedLoopLogFilter : LogFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private ClosedLoopLogFilter(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion)
            : base(
            factory,
            parameter,
            conversion)
        {
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static LogFilter GetInstance(
            LogWriterFactory factory,
            ParameterDatabase database)
        {
            Parameter parameter;
            Conversion conversion;
            if ((database.TryGetParameterById("E3", out parameter) ||
                database.TryGetParameterById("E33", out parameter)) &&
                parameter.TryGetConversionByUnits("status", out conversion))
            {
                return new ClosedLoopLogFilter(factory, parameter, conversion);
            }
            else
            {
                return new LogFilter(factory, null, null);
            }
        }

        /// <summary>
        /// Log when in closed loop
        /// </summary>
        protected override bool ShouldLog(LogColumn column)
        {
            return string.Compare(column.ValueAsString, "8", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    /// <summary>
    /// Full throttle logging
    /// </summary>
    public class FullThrottleLogFilter : LogFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private FullThrottleLogFilter(
            LogWriterFactory factory,
            Parameter parameter,
            Conversion conversion)
            : base(
            factory,
            parameter,
            conversion)
        {
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static LogFilter GetInstance(
            LogWriterFactory factory,
            ParameterDatabase database)
        {
            Parameter parameter;
            Conversion conversion;
            if (database.TryGetParameterById("P13", out parameter) &&
                parameter.TryGetConversionByUnits("%", out conversion))
            {
                return new FullThrottleLogFilter(factory, parameter, conversion);
            }
            else
            {
                return new LogFilter(factory, null, null);
            }
        }

        /// <summary>
        /// Log when throttle is maxed out
        /// </summary>
        protected override bool ShouldLog(LogColumn column)
        {
            return (column.ValueAsDouble > 98.0d);
        }
    }
}
