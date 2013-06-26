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
        public static TableAxis CreateAxis(XElement xel, Table table)
        {
            if (table.IsBase)
                return CreateBaseAxis(xel, table);
            else
            {
                if (table.GetType() == typeof(Table2D))
                    return CreateChildAxis(xel, (Table2D)table);
                else if (table.GetType() == typeof(Table3D))
                    return CreateChildAxis(xel, (Table3D)table);
                else
                    throw new Exception("Error bad table: " + xel.ToString());
            }
        }

        public static TableAxis CreateBaseAxis(XElement xel, Table table)
        {
            if (xel.Attribute("type") != null)
            {
                if (xel.Attribute("type").Value.ToString().ContainsCI("static"))
                {
                    //TODO Add function to handle X or Y static?
                    return new StaticAxis(xel, table, null);
                }
                else if (xel.Attribute("type").Value.ToString().ContainsCI("y"))
                {
                    return new YAxis(xel, table, null);
                }
                else
                {
                    return new XAxis(xel, table, null);
                }
            }

            throw new Exception("Error bad xml for axis: " + xel.ToString());
        }

        public static TableAxis CreateChildAxis(XElement xel, Table3D table)
        {
            if (xel.Attribute("name") != null)
            {
                Table3D bt = (Table3D)table.BaseTable;
                string name = xel.Attribute("name").Value.ToString();

                if (name.ContainsCI("y"))
                {
                    return bt.yAxis.ConstructChild(xel,table);
                }
                else
                {
                    return bt.xAxis.ConstructChild(xel,table);
                }
            }
            else
                throw new Exception("Error bad xml for axis: " + xel.ToString());
        }

        public static TableAxis CreateChildAxis(XElement xel, Table2D table)
        {
            if (xel.Attribute("name") != null)
            {
                Table2D bt = (Table2D)table.BaseTable;
                return bt.Axis.ConstructChild(xel, table);
            }
            else
                throw new Exception("Error bad xml for axis: " + xel.ToString());
        }

    }

    public class TableAxis
    {
        private bool isXAxis { get; set; }

        public bool isStatic { get; protected set; }

        public int Address { get; protected set; }

        public int Elements { get { if (elements != null && elements != 0) return elements; else return BaseAxis.Elements; } protected set { } }
        protected int elements;

        public string Name { get { if(IsBase) return name; else return BaseAxis.Name;}  set{} }
        protected string name;

        public string Type { get { if (IsBase) return name; else return BaseAxis.Name; } private set { } }
        protected string type;

        public string endian { get; private set; }

        private List<float> floatList { get; set; }

        public List<string> staticList { get; private set; }

        public Dictionary<string, string> properties { get; set; }

        public List<byte[]> byteValues { get; set; }

        public List<float> floatValues { get; set; }

        public List<string> displayValues { get; set; }

        public Scaling defaultScaling { get; set; }

        public Scaling scaling { get; set; }

        public string units { get; set; }

        public string frexpr { get; set; }

        public float min { get; set; }

        public float max { get; set; }

        public float inc { get; set; }

        public Table parentTable { get; private set; }

        public DataTable dataTable { get; set; }

        public TableAxis BaseAxis { get; protected set; }

        protected bool IsBase { get; set; }

        /// <summary>
        /// Constructor from XElement
        /// </summary>
        /// <param name="xel"></param>
        public TableAxis(XElement xel, Table table, TableAxis tax)
        {
            if (tax != null)
            {
                IsBase = false;
                BaseAxis = tax;
            }
            else
                IsBase = true;

            properties = new Dictionary<string, string>();
            Name = "base";
            Elements = new int();
            Address = new int();
            staticList = new List<string>();
            floatList = new List<float>();
            parentTable = table;

            if (xel != null)
            {
                foreach (XAttribute attribute in xel.Attributes())
                {
                    this.properties.Add(attribute.Name.ToString(), attribute.Value.ToString());
                    switch (attribute.Name.ToString())
                    {
                        case "name":
                            this.name = attribute.Value.ToString();
                            continue;

                        case "address":
                            this.Address = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                            continue;

                        case "elements":
                            if (System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.Integer) != 0)
                                this.elements = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.Integer);
                            else
                            {
                                int lol = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.Integer);//just for breakpoint
                            }
                            continue;
                    }
                }
                //TODO: DECIDE WHICH ATTRIBUTES CAN BE OVERRIDDEN!!
                if (IsBase)
                {
                    foreach (XAttribute attribute in xel.Attributes())
                    {
                        switch (attribute.Name.ToString())
                        {

                            case "endian":
                                this.endian = attribute.Value.ToString();
                                continue;

                            case "units":
                                this.units = attribute.Value.ToString();
                                continue;

                            case "frexpr":
                                this.frexpr = attribute.Value.ToString();
                                continue;

                            case "min":
                                this.min = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.Float);
                                continue;

                            case "max":
                                this.max = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.Float);
                                continue;

                            case "inc":
                                this.inc = System.Int32.Parse(attribute.Value.ToString(), System.Globalization.NumberStyles.Float);
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
                }
            }
        }

        public virtual TableAxis ConstructChild(XElement xel, Table table)
        {
            return new TableAxis(xel, table, this);
        }

        public virtual void Read(DeviceImage image)
        {

            //TODO, pull this ot and make as extension to generic axis
            //remove "scaling" reference -> populate object during construction
            this.scaling = this.defaultScaling;

            lock (image.imageStream)
            {
                image.imageStream.Seek(this.Address, SeekOrigin.Begin);
                this.byteValues = new List<byte[]>();
                this.floatValues = new List<float>();
                this.displayValues = new List<string>();

                for (int i = 0; i < this.Elements; i++)
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
            DeviceImage image = this.parentTable.parentImage;
            lock (image.imageStream)
            {
                image.imageStream.Seek(this.Address, SeekOrigin.Begin);

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


    public class YAxis : TableAxis
    {
        public YAxis(XElement xel, Table table, TableAxis ax)
            : base(xel, table, ax)
        {
        }

        public override TableAxis ConstructChild(XElement xel, Table table)
        {
            return new YAxis(xel, table, this);
        }
    }

    public class XAxis : TableAxis
    {
        public XAxis(XElement xel, Table table, TableAxis ax)
            : base(xel, table, ax)
        {
        }

        public override TableAxis ConstructChild(XElement xel, Table table)
        {
            return new YAxis(xel, table, this);
        }

    }
    /// <summary>
    /// Static Axes SubClass
    /// </summary>
    public class StaticAxis : TableAxis
    {
        protected List<float> StaticData { get; set; }

        public StaticAxis(XElement xel, Table table, TableAxis ax)
            : base(xel, table, ax)
        {
            isStatic = true;
            if (xel != null)
            {
                staticList.Clear();
                foreach (XElement child in xel.Elements())
                {
                    string cname = child.Name.ToString();

                    switch (cname)
                    {
                        case "data":
                            this.staticList.Add(child.Value.ToString());
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public override TableAxis ConstructChild(XElement xel, Table table)
        {
            return new StaticAxis(xel, table, this);
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
            return this.Name;
        }
    }

}
