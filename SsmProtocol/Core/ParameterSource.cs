///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// ParameterSource.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace NateW.Ssm
{
    [CLSCompliant(true)]
    public class ParameterSource
    {
        private string name;

        private List<Parameter> parameters;

        private PropertyBag propertyBag;

        public ReadOnlyCollection<Parameter> Parameters
        {
            get
            {
                return this.parameters.AsReadOnly();
            }
        }

        public PropertyBag PropertyBag
        {
            get
            {
                return this.propertyBag;
            }
        }

        protected ParameterSource(string name)
        {
            this.name = name;
            this.propertyBag = new PropertyBag();
            this.parameters = new List<Parameter>();
        }

        protected void AddParameter(Parameter parameter)
        {
            this.parameters.Add(parameter);
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}