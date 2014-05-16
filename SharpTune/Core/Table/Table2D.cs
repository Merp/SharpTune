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
using SharpTuneCore;
using SharpTune.Core;
using System.Runtime.Serialization;

namespace SharpTuneCore
{
    public class Table2D : Table
    {

        public Table2D(XElement xel,Definition def, Table basetable)
            : base(xel, def, basetable)
        {
            this.type = "2D";
        }

        public override Table CreateChild(Lut ilut,Definition d)
        {
            //TODO: This is a major KLUDGE.
            if (ilut.GetType() != typeof(Lut2D))
                return base.CreateChild(ilut, d);

            XElement xel;
            Lut2D lut = (Lut2D)ilut;
            xel = new XElement("table");
            xel.SetAttributeValue("name", name);
            xel.SetAttributeValue("address", ilut.dataAddress.ToString("X"));
            if (this.xAxis != null)
            {
                XElement tx = new XElement("table");
                tx.SetAttributeValue("name", "X");
                tx.SetAttributeValue("address", lut.colsAddress.ToString("X"));
                tx.SetAttributeValue("elements", lut.cols);
                xel.Add(tx);
            }
            else
            {
                XElement ty = new XElement("table");
                ty.SetAttributeValue("name", "Y");
                ty.SetAttributeValue("address", lut.colsAddress.ToString("X"));
                ty.SetAttributeValue("elements", lut.cols);
                xel.Add(ty);
            }
            return TableFactory.CreateTable(xel, name, d);
            //TODO also set attirbutes and split this up! Copy to table2D!!
            //return base.CreateChild(lut,d);
            //TODO FIX?? AND CHECK FOR STATIC AXES!!
        }

        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = this.yAxis.elements;    // * this.yAxis.elements;TODO WTF

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

        public override void Write()
        {
            DeviceImage image = this.parentImage;
            lock (image.imageStream)
            {
                //2D only has Y axis
                this.yAxis.Write();
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

    public class RamTable2D : Table2D
    {

        public RamTable2D(XElement xel,Definition def, Table basetable)
            : base(xel,def,basetable)
        {

        }

    }
}
