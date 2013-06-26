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
using SharpTune.GUI;
using SharpTune;

namespace SharpTuneCore
{

    public class Table3D : Table
    {
        public TableAxis xAxis { get; protected set; }
        public TableAxis yAxis { get; protected set; }

        public Table3D(XElement xel, Definition d, Table t)
            : base(xel, d, t)
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
                        this.Description = child.Value.ToString();
                        break;

                    default:
                        break;
                }
            }
            if (xAxis == null)
            {
                Table3D tt = (Table3D)BaseTable;
                if (tt.xAxis != null)
                    xAxis = tt.xAxis.ConstructChild(null, this);
                else
                    throw new Exception(String.Format("Table3D {0} is missing xAxis!", Name));
            }
            if (yAxis == null)
            {
                Table3D tt = (Table3D)BaseTable;
                if (tt.yAxis != null)
                    yAxis = tt.yAxis.ConstructChild(null, this);
                else
                    throw new Exception(String.Format("Table3D {0} is missing yAxis!", Name));
            }
            //TODO
            //EVERY table gets its own axis!! OK to base it on the base table!
            //Check the axis size has not been updated! add size???
            //When exporting, check the axis size!//todo: replace the bool with table, if t = null, iTSA BASE TABLE
        }

        public override Table ConstructChild(XElement xel, Definition d)
        {
            return new Table3D(xel, d, this);
        }

        public void AddAxis(XElement axis)
        {
            string type;
            if (axis.Attribute("type") != null)
                type = axis.Attribute("type").Value.ToString();
            else if (axis.Attribute("name") != null)
                type = axis.Attribute("name").Value.ToString();
            else
                throw new Exception("Error bad table: " + axis.ToString());
            
            if (type.ContainsCI("y"))
                this.yAxis = AxisFactory.CreateAxis(axis, this);
            else
                this.xAxis = AxisFactory.CreateAxis(axis, this);

        }
        public override Table CreateChild(Lut ilut,Definition d)
        {
            Lut3D lut = (Lut3D)ilut;
            xml = new XElement("table");
            xml.SetAttributeValue("name", Name);
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

        /// <summary>
        /// Read table bytes from ROM
        /// And convert to display values
        /// </summary>
        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = this.xAxis.Elements * this.yAxis.Elements;
            this.defaultScaling = SharpTuner.DataScalings.Find(s => s.name.ToString().Contains(this.properties["scaling"].ToString()));
            this.scaling = this.defaultScaling;

            lock (image.imageStream)
            {
                image.imageStream.Seek(this.Address, SeekOrigin.Begin);
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

        public override string GetHEWScript()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(MakeHewName(Name, Address.ToHexString0x()));

            if (xAxis != null && !xAxis.isStatic)
                builder.AppendLine(MakeHewName(Name + "_X_AXIS", xAxis.Address.ToHexString0x()));
            if (yAxis != null && !yAxis.isStatic)
                builder.AppendLine(MakeHewName(Name + "_Y_AXIS", yAxis.Address.ToHexString0x())); //TODO: use UTILS for IDA name formatting

            builder.AppendLine(MakeHEWReferenceScript(Address.ToHexString0x(), Name, 12));

            return builder.ToString();
        }

        public override List<AxisDefinitionControl> GenerateAxisControls()
        {
            List<AxisDefinitionControl> lad = new List<AxisDefinitionControl>();
            lad.Add(new AxisDefinitionControl(xAxis));
            lad.Add(new AxisDefinitionControl(yAxis));
            return lad;
        }

    }

    public class RamTable3D : Table3D
    {

        public RamTable3D(XElement xel, Definition d, Table t)
            : base(xel,d,t)
        {

        }

    }
}

