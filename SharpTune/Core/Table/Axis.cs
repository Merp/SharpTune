/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharpTune;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace SharpTuneCore
{

    public class AxisFactory
    {
        /// <summary>
        /// Handles creation of different table types
        /// Passes XElement to the proper table
        /// </summary>
        /// <param name="xel"></param>
        /// <returns></returns>
        public static Axis CreateAxis(XElement xel, Table table)
        {
            if (xel.Attribute("type") != null)
            {
                if (xel.Attribute("type").Value.ToString().ContainsCI("static"))
                {
                    //TODO Add function to handle X or Y static?
                    return new StaticYAxis(xel, table);
                }
                else if (xel.Attribute("type").Value.ToString().ContainsCI("y"))
                {
                    return new YAxis(xel, table);
                }
                else
                {
                    return new XAxis(xel, table);
                }
            }

            return new Axis(xel, table);
        }

        public static Axis CreateAxis(XElement axis1, Table table, Axis axis2)
        {
            Axis axis = CreateAxis(axis1, table);
            axis.parentAxis = axis2;
            return axis;
        }
    }

    public class Axis : Table
    {

        public Axis parentAxis; //todo: resolve accessibility!

        private bool isXAxis { get; set; }

        public bool isStatic { get; private set; }

        private List<float> floatList { get; set; }

        public List<string> staticList { get; private set; }

        public DataTable dataTable { get; set; }

        /// <summary>
        /// Constructor from XElement
        /// </summary>
        /// <param name="xel"></param>
        public Axis(XElement xel, Table table)
        {

            this.name = "base";
            this.elements = new int();
            this.address = new int();
            this.type = "generic axis";
            this.staticList = new List<string>();
            this.floatList = new List<float>();
            this.baseTable = table;


            //try
            //{

            foreach (XAttribute attribute in xel.Attributes())
            {

                switch (attribute.Name.ToString())
                {
                    case "name":
                        if (parentAxis != null && parentAxis.name != null)
                            break;
                        this.name = attribute.Value.ToString();
                        continue;

                    case "address":
                        this.address = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                        continue;

                    case "elements":
                        this.elements = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.Integer);
                        continue;

                    case "scaling":
                        this.defaultScaling = new Scaling();
                        this.defaultScaling = SharpTuner.DataScalings.Find(s => s.name == attribute.Value.ToString());
                        //TODO FIX: this.endian = this.defaultScaling.endian;
                        continue;

                    case "type":
                        this.type = attribute.Value.ToString();
                        if (this.type.ToString().Contains("static"))
                        {
                            this.isStatic = true;
                        }
                        continue;

                    default:
                        continue;
                }
            }

            foreach (XElement child in xel.Elements())
            {
                string name = child.Name.ToString();

                switch (name)
                {
                    case "data":
                        this.staticList.Add(child.Value.ToString());
                        break;

                    default:
                        break;
                }
            }
            //}
            //catch (System.Exception excpt)
            //{
            //    Trace.WriteLine(excpt.Message);
            //}

        }

        public virtual void Read(DeviceImage image)
        {

            //TODO, pull this ot and make as extension to generic axis
            //remove "scaling" reference -> populate object during construction
            this.scaling = this.defaultScaling;

            lock (image.imageStream)
            {
                image.imageStream.Seek(this.address, SeekOrigin.Begin);
                this.byteValues = new List<byte[]>();
                this.floatValues = new List<float>();
                this.displayValues = new List<string>();

                for (int i = 0; i < this.elements; i++)
                {
                    if (this.isStatic)
                    {
                        this.displayValues = this.staticList;
                        break;
                    }
                    byte[] b = new byte[this.scaling.storageSize];
                    image.imageStream.Read(b, 0, this.scaling.storageSize);
                    if (this.endian == "big")
                    {
                        b.ReverseBytes();
                    }
                    this.byteValues.Add(b);
                    this.displayValues.Add(this.scaling.toDisplay(b));
                    //Must add scaling stuff in here!
                }
            }
        }

        public virtual void Write()
        {
            DeviceImage image = this.baseTable.parentImage;
            lock (image.imageStream)
            {
                image.imageStream.Seek(this.address, SeekOrigin.Begin);

                //write this.bytevalues!
                foreach (byte[] bytevalue in this.byteValues)
                {
                    if (this.scaling.endian == "big")
                    {
                        bytevalue.ReverseBytes();
                    }
                    image.imageStream.Write(bytevalue, 0, bytevalue.Length);
                }
            }

        }

        public virtual string GetScalingDisplayName()
        {
            return this.scaling.name;
        }
    }


    public class YAxis : Axis
    {
        public YAxis(XElement xel, Table table)
            : base(xel, table)
        {
        }





    }

    public class XAxis : Axis
    {
        public XAxis(XElement xel, Table table)
            : base(xel, table)
        {
        }

    }
    /// <summary>
    /// Static Axes SubClass
    /// </summary>
    public class StaticYAxis : YAxis
    {

        protected List<float> StaticData { get; set; }

        public StaticYAxis(XElement xel, Table table)
            : base(xel, table)
        {

        }

        public override void Read(DeviceImage image)
        {

            //TODO, pull this ot and make as extension to generic axis
            //remove "scaling" reference -> populate object during construction
            this.scaling = this.defaultScaling;
            this.displayValues = new List<string>();
            foreach (string value in this.staticList)
            {
                this.displayValues.Add(value.ToString());
            }

        }

        public override void Write()
        {
            //do nothing!
            return;
        }



        public override string GetScalingDisplayName()
        {
            return this.name;
        }
    }

}
