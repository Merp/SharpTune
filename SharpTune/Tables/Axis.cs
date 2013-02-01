using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ModRom.Tables
{
    public class Axis
    {
        private bool isXAxis { get; set; }

        private bool isStatic { get; set; }

        private List<float> floatList { get; set; }

        private List<string> staticList { get; set; }

        //private Scaling scaling { get; set; }

        /// <summary>
        /// Constructor from XElement
        /// </summary>
        /// <param name="xel"></param>
        public Axis(XElement xel)
        {

        }
    }

    /// <summary>
    /// Static Axes SubClass
    /// </summary>
    public class StaticAxis : Axis
    {

        protected List<float> StaticData { get; set; }

        public StaticAxis(XElement xel)
            : base(xel)
        {

        }

    }
}
