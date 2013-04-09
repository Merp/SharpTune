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
using System.Xml.XPath;
using System.Xml.Linq;
using System.Collections;
using System.Data;
using SharpTune;
using System.Windows.Forms;
using System.Drawing;
using SharpTune.Core;
using System.Runtime.Serialization;


namespace SharpTuneCore
{

    public class TableFactory
    {

        /// <summary>
        /// Handles creation of different table types
        /// Passes XElement to the proper table
        /// </summary>
        /// <param name="xel"></param>
        /// <returns></returns>
        public static Table CreateTable(XElement xel, Definition d)
        {
            bool b = false;
            if (xel.Attribute("address") == null)
                b = true;
            return CreateTableWithDimension(xel, d, b);
        }

        public static Table CreateTableWithDimension(XElement xel, Definition d, bool b)
        {
            if (xel.Attribute("type") != null)
            {
                switch (xel.Attribute("type").Value.ToString())
                {
                    case "1D":
                        return new Table1D(xel,d,b);
                    case "2D":
                        return new Table2D(xel,d,b);
                    case "3D":
                        return new Table3D(xel,d,b);
                    default:
                        break;
                }
            }
            return new Table(xel,d,b);
        }

        public static Scaling NewScalingHandler(XElement xel)
        {
            if (xel.Attribute("storagetype") != null)
            {
                return ScalingFactory.CreateScaling(xel);
            }
            return null;
        }
    }




    public enum TableType
    {
        Float = 0x00,
        UInt8 = 0x04,
        UInt16 = 0x08,
        Int8 = 0x0C,
        Int16 = 0x10
    }

    public class Table
    {
        public XElement xml { get; set; }//TODO WRITE SETTER
        public DataTable dataTable { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string Tag { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public string tableTypeString { get; set; }
        public TableType tableTypeHex { get; set; }
        //public string scalingName { get; set; }
        public int level { get; set; }
        public int address { get; set; }
        private int dataScaling { get; set; }
        private int colorMin { get; set; }
        private int colorMax { get; set; }
        public int elements { get; set; }
        public Axis xAxis { get; set; }
        public Axis yAxis { get; set; }
        public Dictionary<string, string> properties { get; set; }
        public string endian { get; private set; }
        public List<byte[]> byteValues { get; set; }
        public List<float> floatValues { get; set; }
        public List<string> displayValues { get; set; }
        public Scaling defaultScaling { get; set; }
        public Scaling scaling { get; set; }
        public DeviceImage parentImage { get; private set; }
        public List<string> Attributes { get; set; }
        public bool isBase { get; private set; }

        public List<Table> InheritanceList { get; private set; }

        public Table()
        {
            //TODO INIT STUFF
        }

        public virtual Table CreateChild(Lut lut, Definition d)
        {
            XElement xml = new XElement("table");
            xml.SetAttributeValue("name", name);
            xml.SetAttributeValue("address", lut.dataAddress.ToString("X"));
            return TableFactory.CreateTable(xml, d);
            //TODO also set attirbutes and split this up!
        }

        /// <summary>
        /// Method to parse XML for adding a table axis
        /// </summary>
        public void AddAxis()
        {
        }

        /// <summary>
        /// Construct from XML Element
        /// </summary>
        /// <param name="xel"></param>
        public Table(XElement xel, Definition d, bool b)
        {
            InheritanceList = new List<Table>();
            isBase = b;
            if(xel.Attribute("name") != null)
                name = xel.Attribute("name").Value.ToString();
            if (d != null && d.inheritList != null)
            {
                foreach (Definition id in d.inheritList)
                {
                    if (id.RomTableList.ContainsKey(name))
                    {
                        if (id.RomTableList[name].InheritanceList != null)
                            InheritanceList.AddRange(id.RomTableList[name].InheritanceList);

                        InheritanceList.Add(id.RomTableList[name]);
                        break;
                    }
                }
                    
            }

            xml = xel;

            dataTable = new DataTable();

            properties = new Dictionary<string, string>();

            foreach (XAttribute attribute in xel.Attributes())
            {
                this.properties.Add(attribute.Name.ToString(), attribute.Value.ToString());
            }

            if (!this.properties.ContainsKey("name"))
            {
                this.properties.Add("name", "unknown");
            }

            else
            {
                this.name = this.properties["name"].ToString();
                this.Tag = this.name + ".table";
            }

            foreach (KeyValuePair<string, string> property in this.properties)
            {
                switch (property.Key.ToString())
                {
                    case "name":
                        this.name = property.Value.ToString();
                        continue;

                    case "address":
                        this.address = System.Int32.Parse(property.Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                        continue;

                    case "elements":
                        this.elements = System.Int32.Parse(property.Value.ToString(), System.Globalization.NumberStyles.Integer);
                        continue;

                    case "scaling":
                        this.scaling = new Scaling();
                        this.scaling = SharpTuner.DataScalings.Find(s => s.name == property.Value.ToString());
                        continue;

                    case "type":
                        this.type = property.Value.ToString();
                        continue;

                    default:
                        continue;
                }
            }

            foreach (XElement child in xel.Elements())
            {
                string cname = child.Name.ToString();

                switch (cname)
                {
                    case "table":
                        this.AddAxis(child);
                        break;

                    case "description":
                        this.description = child.Value.ToString();
                        break;

                    default:
                        break;
                }
            }
        }

        public void ReadProperties(List<Scaling> scalinglist)
        {

            foreach (KeyValuePair<string, string> property in this.properties)
            {
                switch (property.Key)
                {
                    case "name":
                        this.name = property.Value;
                        this.Tag = this.name + ".table";
                        break;
                    case "category":
                        this.category = property.Value;
                        break;
                    case "type":
                        this.tableTypeString = property.Value;
                        break;
                    case "level":
                        this.level = System.Int32.Parse(property.Value.ToString());
                        break;
                    case "scaling":
                        this.defaultScaling = scalinglist.Find(s => s.name == property.Value);
                        break;
                    case "address":
                        this.address = System.Int32.Parse(property.Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                        break;
                    default:
                        break;
                }
            }
        }

        public void AddAxis(XElement axis)
        {
            if (axis.Attribute("type") != null)
            {
                if (axis.Attribute("type").Value.ToString().ContainsCI("y"))
                {
                    //Adding new X axis
                    this.yAxis = AxisFactory.CreateAxis(axis, this);

                }
                else
                {
                    this.xAxis = AxisFactory.CreateAxis(axis, this);

                }
                // else
                {
                }
            }


        }

        public virtual Table MergeTables(Table basetable)
        {
            foreach (KeyValuePair<string, string> property in basetable.properties)
            {
                //If property doesn't exist in the child, add it from the base!
                if (!this.properties.ContainsKey(property.Key))
                {
                    this.properties.Add(property.Key, property.Value);
                }
            }

            return this;
        }

        public virtual void Read()
        {
        }

        public virtual void Write()
        {
        }
    }


}
