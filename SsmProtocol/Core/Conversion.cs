///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Conversion.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using EB.Math;

namespace NateW.Ssm
{
    /// <summary>
    /// Converts values to strings suitable for display
    /// </summary>
    [CLSCompliant(true)]
    public class Conversion
    {
        /// <summary>
        /// Default conversion if none found in logger.xml
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public readonly static Conversion Raw = new Conversion("raw", "x", "0.00");

        /// <summary>
        /// 'Units' string for boolean parameters (aka switches)
        /// </summary>
        public const string Boolean = "boolean";

        /// <summary>
        /// Value string for nonzero Boolean.
        /// </summary>
        public const string BooleanTrue = "On";

        /// <summary>
        /// Value string for zero Boolean.
        /// </summary>
        public const string BooleanFalse = "Off";

        /// <summary>
        /// Units the conversion yeilds
        /// </summary>
        private string units;

        /// <summary>
        /// String containing the mathematical expression for the conversion
        /// </summary>
        private string expression;

        /// <summary>
        /// Format to use for display of the value
        /// </summary>
        private string format;

        /// <summary>
        /// Function that executes the expression
        /// </summary>
        private Function function;

        /// <summary>
        /// Units the conversion yeilds
        /// </summary>
        public string Units
        {
            [DebuggerStepThrough()] 
            get { return units; }
        }

        /// <summary>
        /// String containing the mathematical expression for the conversion
        /// </summary>
        public string Expression 
        {
            [DebuggerStepThrough()]
            get { return expression; }
        }

        /// <summary>
        /// Format to use for display of the value
        /// </summary>
        public string Format 
        {
            [DebuggerStepThrough()]
            get { return format; }
        }

        /// <summary>
        /// Constructor (private, use factory instead)
        /// </summary>
        private Conversion(string units, string expression, string format)
        {
            this.units = units;
            this.expression = expression;
            this.format = "{0:" + format + "}";
        }

        /// <summary>
        /// For display in debugger, do not use in UI
        /// </summary>
        [DebuggerStepThrough()]
        public override string ToString()
        {
            return this.units + " (" + this.format + ")";
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static Conversion GetInstance(string units, string expression, string format)
        {
            return new Conversion(units, expression, format);
        }

        /// <summary>
        /// Convert and format the given value 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        public void Convert(double value, out string valueAsString, out double valueAsDouble)
        {
            if (this.function == null)
            {
                this.function = new Function();
                function.Parse(this.expression);
                function.Infix2Postfix();
            }

            ArrayList var = function.Variables;
            Symbol symbol = (Symbol)var[0];
            symbol.m_value = value;
            var[0] = symbol;
            function.Variables = var;
            function.EvaluatePostfix();
            valueAsDouble = function.Result;
            
            if (this.units == Conversion.Boolean)
            {
                if (valueAsDouble == 0)
                {
                    valueAsString = Conversion.BooleanFalse;
                }
                else
                {
                    valueAsString = Conversion.BooleanTrue;
                    valueAsDouble = 1;
                }
            }
            else
            {
                valueAsString = string.Format(CultureInfo.InvariantCulture, format, valueAsDouble);
            }
        }
        
        /// <summary>
        /// Combine and convert the values in the dependent columns
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public void Convert(
            Dictionary<string, LogColumn> dependencyMap,
            out string valueAsString,
            out double valueAsDouble)
        {
            if (this.function == null)
            {
                this.function = new Function();
                function.Parse(this.expression);
                function.Infix2Postfix();
            }

            ArrayList var = function.Variables;
            foreach (Symbol symbol in var)
            {
                string s = dependencyMap[symbol.m_name].ValueAsString;
                double d = double.Parse(s, CultureInfo.InvariantCulture);
                symbol.m_value = d;
            }
            function.Variables = var;

            function.EvaluatePostfix();
            valueAsDouble = function.Result;
            valueAsString = string.Format(CultureInfo.InvariantCulture, format, valueAsDouble);
        }
    }
}
