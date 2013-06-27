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
using System.Diagnostics;
using SharpTune.GUI;
using SharpTune;


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
        public static Table CreateRomTable(XElement xel, Definition d)
        {
            string type = null;
            if (xel.Attribute("address") != null)
            {
                if (xel.Attribute("name") != null && d.ExposedBaseRomTables.ContainsKey(xel.Attribute("name").Value.ToString()))
                {
                    return d.ExposedBaseRomTables[xel.Attribute("name").Value.ToString()].ConstructChild(xel, d);
                }
                else
                    throw new Exception("Error parsing definition: " + xel.ToString());
            }
            else if (xel.Attribute("type") != null)
            {
                type = xel.Attribute("type").Value.ToString();
            }
            else
                throw new Exception("Error parsing definition: " + xel.ToString());

            switch (type)
            {
                case "1D":
                    return new Table1D(xel, d, null);
                case "2D":
                    return new Table2D(xel, d, null);
                case "3D":
                    return new Table3D(xel, d, null);
                default:
                    break;
            }
            return new Table(xel, d, null);
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
        public bool IsBase;
        public Table BaseTable { get; protected set; }

        public XElement xml { get; set; }//TODO WRITE SETTER

        public string Name { get { if (IsBase) return name; else return BaseTable.Name; } }
        protected string name;

        public string Type { get { if(IsBase) return type; else return BaseTable.Type;} }
        protected string type;

        public string Category { get { if(IsBase) return category; else return BaseTable.Category;} }
        protected string category;

        public string Description { get { if (description != null) return description; else if (!IsBase && BaseTable != null && BaseTable.Description != null) return BaseTable.Description; else return "Error: No Description"; } protected set { description = value; } }
        protected string description;

        public int Address { get; protected set; }

        public List<Table> InheritanceList { get; private set; }

        public Dictionary<string, string> properties { get; set; }

        protected string tableTypeString { get; set; }
        protected TableType tableTypeHex { get; set; }
        protected string scalingName { get; set; }
        protected int level { get; set; }
        protected int dataScaling { get; set; }
        protected int colorMin { get; set; }
        protected int colorMax { get; set; }


        public DataTable dataTable { get; set; }
        protected int elements { get; set; }
        
        public string endian { get; private set; }
        public List<byte[]> byteValues { get; set; }
        public List<float> floatValues { get; set; }
        public List<string> displayValues { get; set; }
        public Scaling defaultScaling { get; set; }
        public Scaling scaling { get; set; }
        public DeviceImage parentImage { get; private set; }
        public Definition parentDef { get; private set; }

        public Table()
        { }

        /// <summary>
        /// Construct from XML Element
        /// </summary>
        /// <param name="xel"></param>
        public Table(XElement xel, Definition d, Table t)
        {
            xml = xel;

            if (t == null)
            {
                IsBase = true;
                if (!d.CalId.ContainsCI("base"))
                    Trace.TraceWarning("Error: no base table given for NON-BASE table: " + t.name);
            }
            else
                BaseTable = t;

            parentDef = d;

            dataTable = new DataTable();

            properties = new Dictionary<string, string>();

            InheritanceList = new List<Table>();

            if (d != null && d.InheritList != null)
            {
                foreach (Definition id in d.InheritList)
                {
                    if (id.DefinedBaseRomTables.ContainsKey(Name)) //TOOD THIS IS INCOMPATIBLE WITH RAM TABLES
                    {
                        if (id.DefinedBaseRomTables[Name].InheritanceList != null)
                            InheritanceList.AddRange(id.DefinedBaseRomTables[Name].InheritanceList);
                        InheritanceList.Add(id.DefinedBaseRomTables[Name]);
                        break;
                    }
                }

                //foreach (Table table in InheritanceList)
                //{
                //    if (table.Category != null)
                //    {
                //        category = table.Category;
                //        break;
                //    }
                //}
            }

        }

        public virtual void ReadXmlECUFlash()
        {

            foreach (XAttribute attribute in xml.Attributes())
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
            }

            //if (xel.Attribute("name") != null)
            //    this.name = xel.Attribute("name").Value.ToString();

            foreach (KeyValuePair<string, string> property in this.properties)
            {
                switch (property.Key.ToString())
                {
                    case "name":
                        this.name = property.Value.ToString();
                        continue;

                    case "address":
                        this.Address = System.Int32.Parse(property.Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
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

                    case "category":
                        this.category = property.Value.ToString();
                        continue;

                    default:
                        continue;
                }
            }

            foreach (XElement child in xml.Elements())
            {
                string cname = child.Name.ToString();

                switch (cname)
                {
                    case "table":
                        //this.AddAxis(child);
                        break;

                    case "description":
                        this.Description = child.Value.ToString();
                        break;

                    default:
                        break;
                }
            }
        }

        public virtual Table ConstructChild(XElement xel, Definition d)
        {
            return new Table(xel, d, this);
        }

        public virtual Table CreateChild(Lut lut, Definition d)
        {
            XElement xml = new XElement("table");
            xml.SetAttributeValue("name", Name);
            xml.SetAttributeValue("address", lut.dataAddress.ToString("X"));
            return ConstructChild(xml, d);
            //TODO also set attirbutes and split this up!
        }

        /// <summary>
        /// Method to parse XML for adding a table axis
        /// </summary>
        public void AddAxis()
        {
        }

        //public void ReadProperties(List<Scaling> scalinglist)
        //{

        //    foreach (KeyValuePair<string, string> property in this.properties)
        //    {
        //        switch (property.Key)
        //        {
        //            case "name":
        //                this.name = property.Value;
        //                break;
        //            case "category":
        //                this.category = property.Value;
        //                break;
        //            case "type":
        //                this.tableTypeString = property.Value;
        //                break;
        //            case "level":
        //                this.level = System.Int32.Parse(property.Value.ToString());
        //                break;
        //            case "scaling":
        //                this.defaultScaling = scalinglist.Find(s => s.name == property.Value);
        //                break;
        //            case "address":
        //                this.Address = System.Int32.Parse(property.Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        //public virtual Table MergeTables(Table basetable)
        //{
        //    foreach (KeyValuePair<string, string> property in basetable.properties)
        //    {
        //        //If property doesn't exist in the child, add it from the base!
        //        if (!this.properties.ContainsKey(property.Key))
        //        {
        //            this.properties.Add(property.Key, property.Value);
        //        }
        //    }

        //    return this;
        //}

        public virtual TableDefinitionControl GenerateTableControl()
        {
            return new TableDefinitionControl(this);
        }

        public virtual List<AxisDefinitionControl> GenerateAxisControls()
        {
            return null;
        }

        public virtual void Read()
        {
        }

        public virtual void Write()
        {
        }

        public virtual string GetHEWScript()
        {
            return MakeHewName(Name, Address.ToHexString0x());
        }

        protected string MakeHewName(string name, string addr)
        {
            if (addr.Length > 0 && name.Length > 0)
            {
                string command = string.Format("MakeNameEx({0}, \"{1}\", SN_CHECK);",
                                               addr,
                                               name.ToIdaString());
                return command;
            }
            return null;
        }

        protected string MakeHEWReferenceScript(string tableAddress, string tableName, int offset)
        {
            tableName = "Table_" + tableName.ToIdaString();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("referenceAddress = DfirstB(" + tableAddress + ");");
            builder.AppendLine("if (referenceAddress > 0)");
            builder.AppendLine("{");
            builder.AppendLine("    referenceAddress = referenceAddress - " + offset.ToString() + ";");
            string command = string.Format("    MakeNameEx(referenceAddress, \"{0}\", SN_CHECK);", tableName);
            builder.AppendLine(command);
            builder.AppendLine("}");
            builder.AppendLine("else");
            builder.AppendLine("{");
            builder.AppendLine("    Message(\"No reference to " + tableName + "\\n\");");
            builder.AppendLine("}");
            return builder.ToString();
        }
    }


}
