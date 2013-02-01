///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Parameter.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace NateW.Ssm
{
    [CLSCompliant(true)]
    public class Parameter
    {
        /// <summary>
        /// Where this parameter came from (SSM, PLX, etc).
        /// </summary>
        private ParameterSource source;

        /// <summary>
        /// Parameter ID
        /// </summary>
        private string id;

        /// <summary>
        /// Display name
        /// </summary>
        private string name;

        /// <summary>
        /// Conversions available to render the parameter value
        /// </summary>
        private IList<Conversion> conversions;

        /// <summary>
        /// Parameters that this parameter is calculated from
        /// </summary>
        private IList<Parameter> dependencies;

        /// <summary>
        /// Where this parameter came from (SSM, PLX, etc).
        /// </summary>
        public ParameterSource Source
        {
            get
            {
                return this.source;
            }
        }

        /// <summary>
        /// Parameter ID
        /// </summary>
        public string Id
        {
            [DebuggerStepThrough()]
            get { return this.id; }
        }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough()]
            get { return this.name; }
        }

        /// <summary>
        /// If true, the parameter's value is calculated from other parameters
        /// </summary>
        public bool IsCalculated
        {
            [DebuggerStepThrough()]
            get { return (this.dependencies != null); }
        }

        /// <summary>
        /// Conversions available to render the parameter value
        /// </summary>
        public IList<Conversion> Conversions
        {
            [DebuggerStepThrough()]
            get { return this.conversions; }
        }

        /// <summary>
        /// Parameters that this parameter is calculated from
        /// </summary>
        public IList<Parameter> Dependencies
        {
            [DebuggerStepThrough()]
            get { return this.dependencies; }
        }

        /// <summary>
        /// Protected constructor, see derived classes.
        /// </summary>
        protected Parameter(
            ParameterSource source,
            string id,
            string name,
            IEnumerable<Conversion> conversions,
            IEnumerable<Parameter> dependencies)
        {
            this.source = source;
            this.id = id;
            this.name = name;
            this.conversions = new ReadOnlyList<Conversion>(conversions);

            if (dependencies != null)
            {
                this.dependencies = new ReadOnlyList<Parameter>(dependencies);
            }
        }

        /// <summary>
        /// Factory.
        /// </summary>
        public static Parameter GetInstance(
            ParameterSource source, 
            string id, 
            string name, 
            IEnumerable<Conversion> conversions, 
            IEnumerable<Parameter> dependencies)
        {
            return new Parameter(source, id, name, conversions, dependencies);
        }

        /// <summary>
        /// Get a Conversion with the given units
        /// </summary>
        public bool TryGetConversionByUnits(string units, out Conversion conversion)
        {
            foreach (Conversion candidate in this.conversions)
            {
                if (candidate.Units == units)
                {
                    conversion = candidate;
                    return true;
                }
            }
            conversion = null;
            return false;
        }

        /// <summary>
        /// Checks for matching IDs
        /// </summary>
        /// <remarks>
        /// Only here to support checking for duplicates in the parameter database
        /// </remarks>
        public override bool Equals(object obj)
        {
            SsmParameter that = obj as SsmParameter;
            if (that == null)
            {
                return false;
            }
            return this.id == that.id;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <remarks>
        /// Only here to suppress warning that happens after you override Equals
        /// </remarks>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        /// <summary>
        /// For debugger only - do not surface this in the UI
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough()]
        public override string ToString()
        {
            return this.name;
        }
    }
}