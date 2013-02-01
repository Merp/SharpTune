///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// InternalLogProfile.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using NSFW.PlxSensors;

namespace NateW.Ssm
{
    /// <summary>
    /// The mutable part of the logger type
    /// </summary>
    internal class InternalLogProfile
    {
        internal static PropertyDefinition ColumnAddressIndex = new PropertyDefinition("AddressIndex");
        internal static PropertyDefinition ColumnPlxSensorId = new PropertyDefinition("PlxSensorId");

        private LogProfile profile;
        private LogEventArgs logEventArgs;
        private List<int> addresses;
        private ParameterDatabase database;
        private StackTrace stackTrace;

        /// <summary>
        /// Public-facing LogProfile instance upon which this instance of InternalLogProfile was based
        /// </summary>
        public LogProfile LogProfile
        {
            [DebuggerStepThrough()]
            get { return this.profile; }
        }

        /// <summary>
        /// EventArgs to pass to the application
        /// </summary>
        public LogEventArgs LogEventArgs
        {
            [DebuggerStepThrough()]
            get { return this.logEventArgs; }
        }

        /// <summary>
        /// Addresses to query from the ECU
        /// </summary>
        public List<int> Addresses
        {
            [DebuggerStepThrough()]
            get { return this.addresses; }
        }

        /// <summary>
        /// Private constructor, use factory instead
        /// </summary>
        /// <param name="profile"></param>
        private InternalLogProfile(LogProfile profile, ParameterDatabase database)
        {
            if (Debugger.IsAttached)
            {
                this.stackTrace = new StackTrace();
            }

            this.profile = profile;
            this.database = database;

            List<LogColumn> columns = new List<LogColumn>();
            this.AddColumnsAndDependents(columns, this.profile);
            this.addresses = BuildAddressList(columns);

            ReadOnlyCollection<LogColumn> readOnly = columns.AsReadOnly();
            LogRow row = LogRow.GetInstance(readOnly);
            this.logEventArgs = new LogEventArgs(row, profile.UserData);
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static InternalLogProfile GetInstance(LogProfile profile, ParameterDatabase database)
        {
            return new InternalLogProfile(profile, database);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("InternalLogProfile (");
            bool first = true;
            foreach (LogColumn column in this.logEventArgs.Row.Columns)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                builder.Append(column.Parameter.Name);
                first = false;
            }

            builder.Append(")");
            if (this.stackTrace != null)
            {
                builder.AppendLine();
                builder.Append(this.stackTrace.ToString());
            }
            return builder.ToString();
        }

        /// <summary>
        /// Convert raw bytes from the ECU to displayable parameters
        /// </summary>
        internal void StoreSsmValues(byte[] rawData)
        {
            this.StoreDirectSsmValues(rawData);
            this.StoreExternalSensorValues();
            this.StoreCalculatedValues();
        }

        /// <summary>
        /// Store values directly from the SSM bytes.
        /// </summary>
        /// <param name="rawData"></param>
        private void StoreDirectSsmValues(byte[] rawData)
        {
            foreach (LogColumn column in this.logEventArgs.Row.Columns)
            {
                if (!(column.Parameter is SsmParameter))
                {
                    continue;
                }

                if (column.DependencyMap != null)
                {
                    continue;
                }

                int index = (int)column.PropertyBag[ColumnAddressIndex];
                int length = ((SsmParameter)column.Parameter).Length;

                double rawValue = InternalLogProfile.GetSsmValue(rawData, index, length);
                InternalLogProfile.ConvertAndStoreValue(column.Parameter, rawValue, column.Conversion, column);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void StoreCalculatedValues()
        {
            foreach (LogColumn column in this.logEventArgs.Row.Columns)
            {
                if (column.DependencyMap == null)
                {
                    continue;
                }

                string valueAsString;
                double valueAsDouble;

                column.Conversion.Convert(
                    column.DependencyMap,
                    out valueAsString,
                    out valueAsDouble);

                column.ValueAsString = valueAsString;
                column.ValueAsDouble = valueAsDouble;
            }
        }

        /// <summary>
        /// Looks up the Conversion to use for the given parameter
        /// </summary>
        /// <param name="dependency">SsmParameter that some other parameter depends on</param>
        /// <param name="parentConversion">Conversion that the 'other' parameter uses</param>
        /// <param name="dependencyConversion">Conversion to use for the dependency</param>
        /// <param name="dependencyKey">This string identifies the dependency 
        /// ID and conversion, will be used later by the expression evaluator.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.IndexOf(System.String,System.StringComparison)")]
        private static void GetDependencyConversion(
            Parameter dependency,
            Conversion parentConversion,
            out Conversion dependencyConversion,
            out string dependencyKey)
        {
            foreach (Conversion conversion in dependency.Conversions)
            {
                dependencyKey = dependency.Id + ":" + conversion.Units;
                if (parentConversion.Expression.IndexOf(dependencyKey, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    dependencyConversion = conversion;
                    return;
                }
            }

            dependencyKey = dependency.Id;
            dependencyConversion = SsmUtility.First(dependency.Conversions);
        }

        /// <summary>
        /// Build the list of addresses to query from the ECU
        /// </summary>
        private static List<int> BuildAddressList(IList<LogColumn> columns)
        {
            List<int> addresses = new List<int>();

            foreach (LogColumn column in columns)
            {
                SsmParameter parameter = column.Parameter as SsmParameter;
                if (parameter == null)
                {
                    continue;
                }

                if (parameter.IsCalculated)
                {
                    continue;
                }

                int address = parameter.Address;
                int index = 0;
                if (addresses.Contains(address))
                {
                    index = addresses.IndexOf(address);
                }
                else
                {
                    index = addresses.Count;
                    addresses.Add(address);
                    for (int i = 1; i < parameter.Length; i++)
                    {
                        addresses.Add(++address);
                    }
                }
                column.PropertyBag[ColumnAddressIndex] = index;
            }

            return addresses;
        }

        /// <summary>
        /// Build the list of columns for the given profile
        /// </summary>
        private void AddColumnsAndDependents(IList<LogColumn> columns, LogProfile newProfile)
        {
            List<LogColumn> dependencies = new List<LogColumn>();

            foreach (LogColumn column in newProfile.Columns)
            {
                Parameter parameter = column.Parameter;
                Conversion conversion = column.Conversion;

                DependencyMap dependencyMap = null;

                if (parameter.IsCalculated)
                {
                    dependencyMap = new DependencyMap();

                    foreach (Parameter dependency in parameter.Dependencies)
                    {
                        string depencencyKey;
                        Conversion dependencyConversion;

                        InternalLogProfile.GetDependencyConversion(
                             dependency,
                             conversion,
                             out dependencyConversion,
                             out depencencyKey);

                        this.VerifyParameter(dependency);

                        LogColumn dependencyColumn = LogColumn.GetInstance(
                            dependency, 
                            dependencyConversion, 
                            null, 
                            true);
                        dependencyMap[depencencyKey] = dependencyColumn;
                        dependencies.Add(dependencyColumn);
                    }
                }

                this.VerifyParameter(parameter);
                LogColumn newColumn = LogColumn.GetInstance(parameter, conversion, dependencyMap, false);
                columns.Add(newColumn);
            }

            if (dependencies != null)
            {
                foreach (LogColumn dependency in dependencies)
                {
                    columns.Add(dependency);
                }
            }
        }

        /// <summary>
        /// Make sure the given parameter is supported.
        /// </summary>
        private void VerifyParameter(Parameter parameter)
        {
            if (!this.database.DoesParameterExist(parameter.Id))
            {
                // TODO: use an exception that application can safely catch.
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The parameter \"{0}\" is not supported.  Parameter support: \"{1}\"",
                        parameter.Name,
                        this.database.PropertyBag.ToString()));
            }
        }

        /// <summary>
        /// Add columns for external sensors.
        /// </summary>
        private void StoreExternalSensorValues()
        {
            PlxSensors plxSensors = ExternalSensors.GetInstance().PlxSensors;
            if (plxSensors == null)
            {
                return;
            }

            foreach (LogColumn column in this.logEventArgs.Row.Columns)
            {
                PlxParameter parameter = column.Parameter as PlxParameter;
                if (parameter != null)
                {
                    PlxSensorId id = (PlxSensorId)parameter.SensorId;
                    double rawValue = plxSensors.GetValue(id, PlxSensorUnits.Raw);
                    InternalLogProfile.ConvertAndStoreValue(column.Parameter, rawValue, column.Conversion, column);
                }
            }
        }

        /// <summary>
        /// Extract a double value from the array of SSM value bytes.
        /// </summary>
        private static double GetSsmValue(byte[] rawData, int index, int length)
        {
            double rawValue = 0;

            if (rawData.Length < length + index)
            {
                Trace.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "InternalLogProfile.GetConvertedValue: rawData.Length {0}, index {1}, length {2}",
                    rawData.Length,
                    index,
                    length));
            }
            else if (length == 1)
            {
                byte b = rawData[index];
                rawValue = (double)b;
            }
            else if (length == 2)
            {
                UInt16 u = rawData[index];// BitConverter.ToInt16(rawData, index);
                u <<= 8;
                u |= rawData[index + 1];
                rawValue = (double)u;
            }
            else if (length == 4)
            {
                UInt32 l = rawData[index++];
                l <<= 8;
                l |= rawData[index++];
                l <<= 8;
                l |= rawData[index++];
                l <<= 8;
                l |= rawData[index++];
                unsafe
                {
                    rawValue = *((float*)&l);
                }
            }

            return rawValue;
        }

        /// <summary>
        /// Convert the given value and store it on the column
        /// </summary>
        private static void ConvertAndStoreValue(Parameter parameter, double rawValue, Conversion conversion, LogColumn column)
        {
            double valueAsDouble;
            string valueAsString;

            try
            {
                conversion.Convert(rawValue, out valueAsString, out valueAsDouble);
            }
            catch (InvalidOperationException)
            {
                Trace.WriteLine("Evaluation failed for parameter " + parameter.Id + " / " + parameter.Name);
                valueAsString = string.Empty;
                valueAsDouble = 0;
            }

            column.ValueAsDouble = valueAsDouble;
            column.ValueAsString = valueAsString;
        }
    }
}
