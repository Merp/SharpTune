///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmParameterDatabase.cs
///////////////////////////////////////////////////////////////////////////////
//#define XPath
//#define XmlReader

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

namespace NateW.Ssm
{
    /// <summary>
    /// Interprets the logger definition XML
    /// </summary>
    public class ParameterDatabase
    {
        /// <summary>
        /// ECU that this database pertains to
        /// </summary>
        private PropertyBag propertyBag;

        /// <summary>
        /// Parameters.
        /// </summary>
        private List<Parameter> parameters;

        /// <summary>
        /// The ECU that this database was built with
        /// </summary>
        public PropertyBag PropertyBag
        {
            [DebuggerStepThrough()]
            get { return this.propertyBag; }
        }

        /// <summary>
        /// Parameters available to this database's ECU
        /// </summary>
        public ReadOnlyCollection<Parameter> Parameters
        {
            get
            {
                return this.parameters.AsReadOnly();
            }
        }
        
        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        private ParameterDatabase()
        {
            this.propertyBag = new PropertyBag();
            this.parameters = new List<Parameter>();
        }

        /// <summary>
        /// Factory
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ParameterDatabase GetInstance()
        {
            return new ParameterDatabase();
        }

        /// <summary>
        /// For debugging only, do not display in UI
        /// </summary>
        public override string ToString()
        {
            return this.propertyBag.ToString() + " (" + this.Parameters.Count + " parameters)";
        }

        /// <summary>
        /// Look up a parameter by ID
        /// </summary>
        public bool TryGetParameterById(string id, out Parameter parameter)
        {
            foreach (Parameter candidate in this.parameters)
            {
                if (candidate.Id == id)
                {
                    parameter = candidate;
                    return true;
                }
            }
            parameter = null;
            return false;
        }

        public bool DoesParameterExist(string id)
        {
            foreach (Parameter parameter in this.parameters)
            {
                if (parameter.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add parameters from the given source.
        /// </summary>
        public void Add(ParameterSource source)
        {
            foreach (Parameter parameter in source.Parameters)
            {
                this.parameters.Add(parameter);
            }

            this.propertyBag.Add(source.PropertyBag);
        }

        /// <summary>
        /// Remove parameters from the given source.
        /// </summary>
        /// <param name="parameterSource"></param>
        public void Remove(ParameterSource source)
        {
            this.parameters.RemoveAll(delegate(Parameter parameter)
            {
                return parameter.Source == source;
            });

            this.propertyBag.Remove(source.PropertyBag);
        }
    }
}
