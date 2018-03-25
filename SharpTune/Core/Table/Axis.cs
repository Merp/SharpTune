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
using System.Diagnostics;

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
        public static Axis CreateAxis(XElement xel, TableMetaData table)
        {
            if (xel.Attribute("type") != null)
            {
                if (xel.Attribute("type").Value.ToString().ContainsCI("static"))
                    return new StaticAxis(xel, table);
                return new Axis(xel, table);
            }
            return null;
        }

        public static Axis CreateAxis(XElement axis1, TableMetaData table, Axis axis2)
        {
            Axis axis;
            if(axis2.type.ContainsCI("static"))
                axis = new StaticAxis(axis1, table);
            else
                axis = new Axis(axis1, table);

            axis.parentAxis = axis2;
            return axis;
        }
    }

    public class Axis : TableMetaData
    {

        public Axis parentAxis; //todo: resolve accessibility!

        protected bool isXAxis { get; set; }

        protected List<float> floatList { get; set; }

        /// <summary>
        /// Constructor from XElement
        /// </summary>
        /// <param name="xel"></param>
        public Axis(XElement xel, TableMetaData table)
        {
            try
            {
                this.isXAxis = true;
                this.name = "base";
                this.elements = new int();
                this.address = new int();
                this.type = "generic axis";
                this.floatList = new List<float>();
                this.baseTable = table;
                this.parentDef = table.parentDef;

                foreach (XAttribute attribute in xel.Attributes())
                {

                    switch (attribute.Name.ToString())
                    {
                        case "name":
                            if (attribute.Value.ToString().EqualsCI("y"))
                                this.isXAxis = false;
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
                            Scaling sca = new Scaling();
                            if (this.parentDef.ScalingList.TryGetValue(attribute.Value, out sca))
                                this.defaultScaling = sca;
                            else
                                Trace.WriteLine("Error finding scaling " + attribute.Value);
                            continue;
                        //TODO FIX: this.endian = this.defaultScaling.endian;

                        case "type":
                            this.type = attribute.Value.ToString();
                            if (this.type.ToString().ContainsCI("y"))
                            {
                                this.isXAxis = false;
                            }
                            continue;

                        default:
                            continue;
                    }
                }
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error creating axis in " + table);
                Trace.WriteLine("XML: " + xel.ToString());
                throw;
            }
        }

        protected override XElement CreateECUFlashXML(){
            XElement xel = new XElement("table");
            if (isBase)
            {
                xel.SetAttributeValue("name", name);
                xel.SetAttributeValue("type", type);
                xel.SetAttributeValue("elements", elements);
            }
            else
            {
                if (!isXAxis)
                    xel.SetAttributeValue("name", "Y");
                else
                    xel.SetAttributeValue("name", "X");

                if(_address != null)
                    xel.SetAttributeValue("address", addressHexString);

                if (_scaling != null && baseTable.scaling != null && _scaling != baseTable.scaling)//TODO FIX THIS KLUDGE
                    xel.SetAttributeValue("scaling", _scaling);

                if (_elements != null)
                {
                    if (baseTable.elements != null)
                    {
                        if (_elements != baseTable.elements)
                        {//TODO FIX THIS KLUDGE
                            xel.SetAttributeValue("elements", _elements);
                        }
                    }
                }
            }
            return xel;
        }

        public virtual void Read(ECU image)
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
            ECU image = this.baseTable.parentImage;
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

    /// <summary>
    /// Static Axes SubClass
    /// </summary>
    public class StaticAxis : Axis
    {
        protected List<string> StaticData { get; set; }

        public StaticAxis(XElement xel, TableMetaData table)
            : base(xel,table)
        {
            try
            {
                StaticData = new List<string>();
                foreach (XElement child in xel.Elements())
                {
                    string nam = child.Name.ToString();

                    switch (nam)
                    {
                        case "data":
                            this.StaticData.Add(child.Value.ToString());
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error creating static axis in " + table);
                Trace.WriteLine("XML: " + xel.ToString());
                throw;
            }
        }

        protected override XElement CreateECUFlashXML()
        {
            XElement xel = new XElement("table");
            if (isBase)
            {
                xel.SetAttributeValue("name", name);
                xel.SetAttributeValue("type", type);
                xel.SetAttributeValue("elements", elements);
            }
            else
            {
                if (!isXAxis)
                    xel.SetAttributeValue("name", "Y");
                else
                    xel.SetAttributeValue("name", "X");

                if (_address != null)
                    xel.SetAttributeValue("address", _address);

                if (_scaling != null && _scaling != baseTable.scaling)
                    xel.SetAttributeValue("scaling", _scaling);

                if (_elements != null && _elements != baseTable.elements)
                    xel.SetAttributeValue("elements", _elements);
            }
            foreach (string stat in StaticData)
            {
                XElement data = new XElement("data");
                data.SetValue(stat);
                xel.Add(stat);
            }
            return xel;
        }

        public override void Read(ECU image)
        {

            //TODO, pull this ot and make as extension to generic axis
            //remove "scaling" reference -> populate object during construction
            this.scaling = this.defaultScaling;
            this.displayValues = new List<string>();
            foreach (string value in this.StaticData)
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
