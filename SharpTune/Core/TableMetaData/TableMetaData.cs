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
        public static TableMetaData CreateTable(XElement xel,string tablename, ECUMetaData def)
        {
            TableMetaData basetable = null;
            if (def.GetBaseRomTable(tablename, out basetable))
            {
                //has a base table!! therefore not a base!

            }
            if (xel.Attribute("address") == null)
                basetable = null;//sure???

            return CreateTableWithDimension(xel, def, basetable);
        }

        public static TableMetaData CreateTableWithDimension(XElement xel, ECUMetaData def, TableMetaData basetable)
        {
            string type = null;
            if (xel.Attribute("type") != null)
                type = xel.Attribute("type").Value.ToString();
            else if (basetable != null && basetable.type != null)
                type = basetable.type;
            if(type != null)
            {
                switch (type)
                {
                    case "1D":
                        return new Table1DMetaData(xel, def, basetable);
                    case "2D":
                        return new Table2DMetaData(xel, def, basetable);
                    case "3D":
                        return new Table3DMetaData(xel, def, basetable);
                    default:
                        break;
                }
            }
            return new TableMetaData(xel,def, basetable);
        }

        /// <summary>
        /// Handles creation of different table types
        /// Passes XElement to the proper table
        /// </summary>
        /// <param name="xel"></param>
        /// <returns></returns>
        public static TableMetaData CreateRamTable(XElement xel, string tablename, string type, ECUMetaData def)
        {
            TableMetaData basetable = null;
            //if (def.GetBaseRamTable(tablename, out basetable))
            //{
                //has a base table!! therefore not a base!

            //}
            if (xel.Attribute("address") == null)
                basetable = null;//sure???

            return CreateRamTableWithDimension(xel, type, def, basetable);
        }

        public static TableMetaData CreateRamTableWithDimension(XElement xel, string storageType, ECUMetaData def, TableMetaData basetable)
        {
            TableMetaData tempTable = null;
            string type = null;
            if (xel.Attribute("type") != null)
                type = xel.Attribute("type").Value.ToString();
            else if (basetable != null && basetable.type != null)
                type = basetable.type;
            if (type != null)
            {
                switch (type)
                {
                    case "1D":
                        tempTable = new RamTable1DMetaData(xel, def, basetable);
                        break;
                    case "2D":
                        tempTable = new RamTable2DMetaData(xel, def, basetable);
                        break;
                    case "3D":
                        tempTable = new RamTable3DMetaData(xel, def, basetable);
                        break;
                    default:
                        tempTable = new RamTable(xel, def, basetable);
                        break;
                }
            }
            if (tempTable == null)
                tempTable = new RamTable(xel, def, basetable);
            tempTable.storageTypeString = storageType;
            return tempTable;
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




    public enum StorageType //TODO: put this in memorymodel.
    {
        Float = 0x00,
        UInt8 = 0x04,
        UInt16 = 0x08,
        Int8 = 0x0C,
        Int16 = 0x10,
        Unknown = 0xFF
    }

    public class TableMetaData
    {
        public string Tag { get; set; }

        public bool isBase { get; private set; }

        public List<TableMetaData> InheritanceList { get; private set; }

        protected XElement _xml;
        public XElement xml {
            get
            {
                _xml = this.CreateECUFlashXML();
                return _xml;
            }
            set
            {
                _xml = value;
            }
        }

        public Dictionary<string, string> properties { get; set; }

        protected TableMetaData _baseTable;
        public TableMetaData baseTable { 
            get
            {
                return _baseTable;
            }
            set
            {
                if (value != null)
                {
                    _baseTable = value;
                    isBase = false;
                }
                else
                    isBase = true;
            }
        }

        public ECU parentImage { get; private set; }
        public ECUMetaData parentDef { get; protected set; }

        public DataTable dataTable { get; set; }
        public List<byte[]> byteValues { get; set; }
        public List<float> floatValues { get; set; }
        public List<string> displayValues { get; set; }

        public string name { get; set; }

        protected string _type;
        public string type
        {
            get
            {
                if (_type != null)
                    return _type;
                else if (baseTable.type != null)
                    return baseTable.type;
                else
                    return "unknown type";
            }
            protected set { _type = value; }
        }

        protected string _category;
        public string category
        {
            get
            {
                if (_category != null)
                    return _category;
                else if (baseTable.category != null)
                    return baseTable.category;
                else
                    return "unknown category";
            }
            protected set { _category = value; }
        }

        protected string _description;
        public string description
        {
            get
            {
                if (_description != null)
                    return _description;
                else if (baseTable.description != null)
                    return baseTable.description;
                else
                    return "unknown description";
            }
            protected set { _description = value; }
        }

        protected string _storageTypeString;
        public string storageTypeString
        {
            get
            {
                if (_storageTypeString != null)
                    return _storageTypeString;
                else if (baseTable.storageTypeString != null)
                    return baseTable.storageTypeString;
                else
                    return "unknown storageTypeString";
            }
            set { _storageTypeString = value; }
        }

        private StorageType _storageTypeHex;
        public StorageType storageTypeHex
        {
            get
            {
                if (_storageTypeHex != StorageType.Unknown)
                    return _storageTypeHex;
                else if (baseTable.storageTypeHex != StorageType.Unknown)
                    return baseTable.storageTypeHex;
                else
                    return StorageType.Unknown;
            }
            protected set { _storageTypeHex = value; }
        }

        protected int? _level;
        public int? level
        {
            get
            {
                if (_level != null)
                    return _level;
                else if (baseTable != null)
                    return baseTable.level;
                else
                    return null;
            }
            protected set { _level = value; }
        }

        protected int? _address;
        public int address
        {
            get
            {
                if (_address != null)
                    return (int)_address;
                else if (baseTable != null)
                    return baseTable.address;
                else
                {
                    throw new Exception("Error, no address found for table: " + this.name + " in definition: " + this.parentDef.calibrationlId);
                }
            }
            protected set { _address = value; }
        }
        public string addressHexString
        {
            get { return address.ConvertIntToHexString(); }
        }

        protected int? _elements;
        public int? elements
        {
            get
            {
                if (_elements != null)
                    return _elements;
                else if (baseTable != null)
                    return baseTable.elements;
                else
                    return null;
            }
            protected set { _elements = value; }
        }
        
        private Axis _xAxis { get; set; }
        public Axis xAxis {
            get
            {
                if (_xAxis != null)
                    return _xAxis;
                else if(baseTable != null)
                    return baseTable.xAxis;
                else
                    return null;
            }
            private set
            {
                _xAxis = value;
            }
        }

        private Axis _yAxis { get; set; }
        public Axis yAxis
        {
            get
            {
                if (_yAxis != null)
                    return _yAxis;
                else if (baseTable != null)
                    return baseTable.yAxis;
                else
                    return null;
            }
            private set
            {
                _yAxis = value;
            }
        }

        protected string _endian;
        public string endian
        {
            get
            {
                if (_endian != null)
                    return _endian;
                else if (baseTable.endian != null)
                    return baseTable.endian;
                else
                    return "unknown endian";
            }
            protected set { _endian = value; }
        }

        protected Scaling _defaultScaling;
        public Scaling defaultScaling
        {
            get
            {
                if (_defaultScaling != null)
                    return _defaultScaling;
                else if (baseTable.defaultScaling != null)
                    return baseTable.defaultScaling;
                else
                {
                    Trace.WriteLine(String.Format("Error, scaling not found in table: {0} of defintion: {1}", name, parentDef.calibrationlId));
                    return new Scaling();
                }
            }
            protected set { _defaultScaling = value; scaling = value; }
        }

        protected Scaling _scaling;
        public Scaling scaling
        {
            get
            {
                if (_scaling != null)
                    return _scaling;
                else if (defaultScaling != null)
                    return defaultScaling;
                else
                {
                    Trace.WriteLine(String.Format("Error, scaling not found in table: {0} of defintion: {1}", name, parentDef.calibrationlId));
                    return new Scaling();
                }
            }
            set
            {
                _scaling = value;
            }
        } 

        public virtual TableMetaData CreateChild(LookupTable lut, ECUMetaData d)
        {
            XElement xel = new XElement("table");
            xel.SetAttributeValue("name", name);
            xel.SetAttributeValue("address", lut.dataAddress.ToString("X"));
            return TableFactory.CreateTable(xel,name, d);
            //TODO also set attirbutes and split this up!
        }

        /// <summary>
        /// Method to parse XML for adding a table axis
        /// </summary>
        public void AddAxis()
        {
        }


        protected TableMetaData()
        {
            dataTable = new DataTable();
            properties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Construct from XML Element
        /// </summary>
        /// <param name="xel"></param>
        public TableMetaData(XElement xel, ECUMetaData def, TableMetaData bt)
        :this(){
            try
            {
                parentDef = def;
                baseTable = bt;
                xml = xel;

                ParseAttributes(xel);
                ParseChildren(xel);
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error creating table " + this.name);
                Trace.WriteLine("XML: " + xel.ToString());
                Trace.WriteLine(crap.Message);
            }
        }

        private void ParseAttributes(XElement xel){
            try
            {
                foreach (XAttribute attribute in xel.Attributes())
                {
                    switch (attribute.Name.ToString().ToLower())
                    {
                        case "name":
                            this.name = attribute.Value.ToString();
                            continue;

                        case "address":
                            this.address = attribute.Value.ConvertHexToInt();
                            continue;

                        case "elements":
                            this.elements = attribute.Value.ConvertStringToInt();
                            continue;

                        case "scaling":
                            Scaling sca = new Scaling();
                            if (this.parentDef.ScalingList.TryGetValue(attribute.Value, out sca))
                            {
                                this.scaling = sca;
                                this.defaultScaling = sca;
                            }
                            else
                                Trace.WriteLine("Error finding scaling " + attribute.Value);
                            continue;

                        case "type":
                            this.type = attribute.Value.ToString();
                            continue;

                        case "category":
                            this.category = attribute.Value.ToString();
                            continue;

                        default:
                            continue;
                    }
                }
            }
            catch (Exception e)
            {
                Exception ne = new Exception("Error parsing table xml attributes", e);
                throw;
            }
        }

        private void ParseChildren(XElement xel){
            try
            {
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
            catch (Exception e)
            {
                Exception ne = new Exception("Error parsing table xml child elements", e);
                throw;
            }
        }

        public void AddAxis(XElement axis)
        {
            try
            {

                if (this.baseTable != null && axis.Attribute("name") != null)
                {
                    string name = axis.Attribute("name").Value;
                    if (name.EqualsCI("x") || name.ContainsCI("x axis"))
                    {
                        if (baseTable.xAxis != null)// && name.EqualsCI(baseTable.xAxis.name)))
                            this.xAxis = AxisFactory.CreateAxis(axis, this, baseTable.xAxis);
                        else if (baseTable.yAxis != null)
                            this.xAxis = AxisFactory.CreateAxis(axis, this, baseTable.yAxis);
                    }
                    else if (name.EqualsCI("y") || name.ContainsCI("y axis"))
                    {
                        if (baseTable.yAxis != null) //&& name.EqualsCI(baseTable.yAxis.name)))
                            this.yAxis = AxisFactory.CreateAxis(axis, this, baseTable.yAxis);
                        else if (baseTable.xAxis != null)
                            this.yAxis = AxisFactory.CreateAxis(axis, this, baseTable.xAxis);
                    }
                }
                else if (axis.Attribute("type") != null)
                {
                    if (axis.Attribute("type").Value.ToString().ContainsCI("y"))
                        this.yAxis = AxisFactory.CreateAxis(axis, this);
                    else
                        this.xAxis = AxisFactory.CreateAxis(axis, this);
                }
                else if (axis.Attribute("name") != null)
                {
                    string name = axis.Attribute("name").Value;
                    if (name.EqualsCI("x") || name.ContainsCI("x axis"))
                        this.xAxis = AxisFactory.CreateAxis(axis, this);
                    else if (name.EqualsCI("y") || name.ContainsCI("y axis"))
                        this.yAxis = AxisFactory.CreateAxis(axis, this);
                }
            }
            catch (Exception e)
            {
                Exception ne = new Exception("Error adding axis", e);
                throw;
            }
        }

        protected virtual XElement CreateECUFlashXML(){
            XElement xel = new XElement("table");
            xel.SetAttributeValue("name",this.name);
            if (isBase)
            {
                xel.SetAttributeValue("elements",this.elements);
                xel.SetAttributeValue("type",this.type);
                xel.SetAttributeValue("category",this.category);
                xel.SetAttributeValue("scaling",this.scaling.name);
                XElement description = new XElement("description");
                description.SetValue(this.description);
                xel.Add(description);
            }
            else
            {

                xel.SetAttributeValue("address", this.address.ToString("X"));

                if(_elements != null && baseTable.elements != null && _elements != baseTable.elements) //TODO FIX KLUDGE!
                    xel.SetAttributeValue("elements",_elements);

                if(_xAxis != null)
                {
                    xel.Add(_xAxis.CreateECUFlashXML());
                }
                if(_yAxis != null)
                {
                    xel.Add(_yAxis.CreateECUFlashXML());
                }
            }
            return xel;
        }

        public virtual XElement RomRaiderXML()
        {
            return null;
        }
    }

    public class RamTable : TableMetaData
    {
        private XElement RRXML;

        public RamTable(XElement xel, ECUMetaData def, TableMetaData basetable)// DeviceImage image)
            : base(xel, def, basetable)
        {
            RRXML = xel;
        }

        public override XElement RomRaiderXML()
        {
            return RRXML;
        }

    }
}
