///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// LogProfile.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace NateW.Ssm
{
    /// <summary>
    /// Binds a Parameter and a Conversion (but no current value) for LogProfile.
    /// </summary>
    /// <remarks>
    /// TODO: consider just using LogColumn internally.
    /// </remarks>
    /*public class Column
    {
        public readonly Parameter Parameter;
        public readonly Conversion Conversion;

        public Column(Parameter parameter, Conversion conversion)
        {
            this.Parameter = parameter;
            this.Conversion = conversion;
        }
    }*/

    /// <summary>
    /// Holds parameters and their conversions 
    /// </summary>
    public class LogProfile
    {        
        private List<LogColumn> columns;

        private object userData;

        public object UserData
        {
            get { return this.userData; }
            set { this.userData = value; }
        }

        public IList<LogColumn> Columns
        {
            get { return this.columns; }
        }

        private LogProfile()
        {
            this.columns = new List<LogColumn>();
        }

        public static LogProfile CreateInstance()
        {
            return new LogProfile();
        }

        /// <summary>
        /// Load parameter list from file.
        /// </summary>
        /// <remarks>
        /// TODO: indicate what parameters in the file are not supported by the current database.
        /// </remarks>
        public static LogProfile Load(string path, ParameterDatabase database)
        {
            Trace.WriteLine("LogProfile.Load");
            if (database == null)
            {
                Trace.WriteLine("LogProfile.Load: no database?  WTF?");
                return LogProfile.CreateInstance();
            }

            LogProfile profile = null;
            using (Stream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerializedColumn[]));
                SerializedColumn[] serializedColumns = (SerializedColumn[])serializer.Deserialize(stream);

                profile = LogProfile.CreateInstance();
                foreach (SerializedColumn serializedColumn in serializedColumns)
                {
                    Parameter parameter;
                    Conversion conversion;
                    if (serializedColumn == null)
                    {
                        Trace.WriteLine("LogProfile.Load: serializedColumn == null?  WTF?");
                        continue;
                    }

                    if (database.TryGetParameterById(serializedColumn.ParameterId, out parameter) &&
                        parameter.TryGetConversionByUnits(serializedColumn.ConversionUnits, out conversion))
                    {
                        profile.Add(parameter, conversion);
                    }
                }
            }
            Trace.WriteLine("LogProfile.Load: done");
            return profile;
        }

        public bool Contains(Parameter parameter)
        {
            foreach (LogColumn column in this.columns)
            {
                if (column.Parameter == parameter)
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(Parameter parameter, Conversion conversion)
        {
            LogColumn column = LogColumn.GetInstance(parameter, conversion, null, false);
            for (int i = 0; i < this.columns.Count; i++)
            {
                LogColumn candidate = this.columns[i];
                if (string.Compare(
                    parameter.Name, 
                    candidate.Parameter.Name, 
                    StringComparison.OrdinalIgnoreCase) < 0)
                {
                    this.columns.Insert(i, column);
                    return;
                }
            }
            this.columns.Add(column);
        }

        /// <summary>
        /// Add the parameter and conversion to the profile, making sure that 
        /// the given parameter and conversion exists in the database.
        /// </summary>
        /// <remarks>
        /// TODO: Move the check logic into a new ParameterDatabase.Exists() method.
        /// </remarks>
        public void Add(string id, string units, ParameterDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            foreach (Parameter parameter in database.Parameters)
            {
                if (parameter.Id == id)
                {
                    foreach (Conversion conversion in parameter.Conversions)
                    {
                        if (conversion.Units == units)
                        {
                            this.Add(parameter, conversion);
                            return;
                        }
                    }
                    throw new ArgumentException("Conversion " + units + " not supported for parameter " + id);
                }
            }
            throw new ArgumentException("Parameter " + id + " not supported.");
        }

        public IList<Conversion> GetConversions(Parameter parameter)
        {
            List<Conversion> result = new List<Conversion>();
            foreach (LogColumn column in this.columns)
            {
                if (column.Parameter != parameter)
                {
                    continue;
                }

                result.Add(column.Conversion);
            }
            return result;
        }

        public LogProfile Clone()
        {
            LogProfile clone = new LogProfile();
            clone.columns = new List<LogColumn>(this.columns);
            return clone;
        }

        /// <summary>
        /// Save the profile to disk
        /// </summary>
        public void Save(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializedColumn[]));
            using (Stream stream = File.OpenWrite(path))
            {
                stream.SetLength(0);
                List<SerializedColumn> serializedColumns = new List<SerializedColumn>(this.columns.Count);
                foreach (LogColumn column in this.columns)
                {
                    serializedColumns.Add(
                        new SerializedColumn(
                            column.Parameter.Id,
                            column.Parameter.Name,
                            column.Conversion.Units));
                }

                serializer.Serialize(stream, serializedColumns.ToArray());
            }
        }
    }
}
