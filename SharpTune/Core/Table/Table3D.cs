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
using System.Data;
using System.IO;
using SharpTune;
using System.Windows.Forms;
using System.Drawing;
using SharpTune.Core;
using System.Runtime.Serialization;

namespace SharpTuneCore
{

    public class Table3D : Table
    {
        private Scaling xAxisScaling { get; set; }

        public Table3D()
        {
        }

        public Table3D(XElement xel, Definition d, bool b)
            : base(xel, d, b)
        {

        }


        public new Table3D DeepClone()
        {
            Table3D clone = new Table3D();
            clone.xml = new XElement(xml);
            clone.dataTable = dataTable.Clone();
            clone.name = name;
            clone.type = type;
            clone.Tag = Tag;
            clone.category = category;
            clone.description = description;
            clone.tableTypeString = tableTypeString;
            clone.tableTypeHex = tableTypeHex;
            //clone.scalingName { get; set; }
            //clone.level = level;
            //clone.address = address;
            //clone.dataScaling = dataScaling;
            //clone.colorMin = colorMin;
            //clone.colorMax = colorMax;
            //clone.elements = elements;
            //clone.xAxis
            //TODO FINISH THESEclone.yAxis { get; set; }
            //clone.properties = new Dictionary<string, string>(properties);
            // clone.endian = endian;
            // clone.byteValues = new List<byte[]>(byteValues);
            //clone.floatValues = new List<float>(floatValues);
            //clone.displayValues = new List<string>(displayValues);
            //clone.defaultScaling 
            //clone.scaling { get; set; }
            //clone.parentImage = parentImage;
            //clone.Attributes = new List<string>(Attributes);
            return clone;
        }

        public override Table CreateChild(Lut ilut,Definition d)
        {
            Lut3D lut = (Lut3D)ilut;
            xml = new XElement("table");
            xml.SetAttributeValue("name", name);
            xml.SetAttributeValue("address", ilut.dataAddress.ToString("X"));
            XElement tx = new XElement("table");
            tx.SetAttributeValue("name", "X");
            tx.SetAttributeValue("address", lut.colsAddress.ToString("X"));
            tx.SetAttributeValue("elements", lut.cols);
            xml.Add(tx);
            XElement ty = new XElement("table");
            ty.SetAttributeValue("name", "Y");
            ty.SetAttributeValue("address", lut.rowsAddress.ToString("X"));
            ty.SetAttributeValue("elements", lut.rows);
            xml.Add(ty);
            return TableFactory.CreateTable(xml, d);
            //TODO also set attirbutes and split this up! Copy to table2D!!
        }

        public override Table MergeTables(Table basetable)
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

        /// <summary>
        /// Read table bytes from ROM
        /// And convert to display values
        /// </summary>
        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = this.xAxis.elements * this.yAxis.elements;
            this.defaultScaling = SharpTuner.DataScalings.Find(s => s.name.ToString().Contains(this.properties["scaling"].ToString()));
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
                    if (this.scaling.endian == "big")
                    {
                        b.ReverseBytes();
                    }
                    this.byteValues.Add(b);
                    this.displayValues.Add(this.scaling.toDisplay(b));
                }

            }
        }

        /// <summary>
        /// Write the table bytes to ROM
        /// </summary>
        public override void Write()
        {
            DeviceImage image = this.parentImage;

            lock (image.imageStream)
            {
                //2D only has Y axis
                this.yAxis.Write();
                this.xAxis.Write();
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
    }

    public class RamTable3D : Table3D
    {

        public RamTable3D(XElement xel, Definition d, bool b)
            : base(xel,d,b)
        {

        }

    }
}

