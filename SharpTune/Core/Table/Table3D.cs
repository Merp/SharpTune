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

        public Table3D(XElement xel, Definition def, Table basetable)
            : base(xel, def, basetable)
        {
            this.type = "3D";
        }

        public override Table CreateChild(Lut ilut,Definition d)
        {
            XElement xel;
            Lut3D lut = (Lut3D)ilut;
            xel = new XElement("table");
            xel.SetAttributeValue("name", name);
            xel.SetAttributeValue("address", ilut.dataAddress.ToString("X"));
            XElement tx = new XElement("table");
            tx.SetAttributeValue("name", "X");
            tx.SetAttributeValue("address", lut.colsAddress.ToString("X"));
            tx.SetAttributeValue("elements", lut.cols);
            xel.Add(tx);
            XElement ty = new XElement("table");
            ty.SetAttributeValue("name", "Y");
            ty.SetAttributeValue("address", lut.rowsAddress.ToString("X"));
            ty.SetAttributeValue("elements", lut.rows);
            xel.Add(ty);
            return TableFactory.CreateTable(xel, name, d);
            //TODO also set attirbutes and split this up! Copy to table2D!!
        }

        /// <summary>
        /// Read table bytes from ROM
        /// And convert to display values
        /// </summary>
        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = this.xAxis.elements * this.yAxis.elements;

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

        public RamTable3D(XElement xel, Definition def, Table basetable)
            : base(xel,def, basetable)
        {

        }

    }
}

