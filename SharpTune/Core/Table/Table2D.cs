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

        public Table2D(XElement xel, Definition d, Table t)
            :base(xel,d,t)
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
            if (Axis == null)
            {
                Table2D tt = (Table2D)t;
                Axis = tt.Axis.ConstructChild(null, this);
            }

            //TODO
            //EVERY table gets its own axis!! OK to base it on the base table!
            //Check the axis size has not been updated! add size???
            //When exporting, check the axis size!//todo: replace the bool with table, if t = null, iTSA BASE TABLE
        }

        public TableAxis Axis { get; protected set; }

        private void AddAxis(XElement axis)
        {
            if (Axis != null)
                throw new Exception("Error, axis already exists!!");

            if (axis.Attribute("type") != null || axis.Attribute("name") != null)
            {
                this.Axis = AxisFactory.CreateAxis(axis, this);
            }
        }

        public override Table ConstructChild(XElement xel, Definition d)
        {
            return new Table2D(xel, d, this);
        }

        public override Table CreateChild(Lut lut,Definition d)
        {
            return base.CreateChild(lut,d);
            //TODO FIX?? AND CHECK FOR STATIC AXES!!
        }

        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = this.Axis.Elements;    // * this.yAxis.elements;
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

        public override void Write()
        {
            DeviceImage image = this.parentImage;
            lock (image.imageStream)
            {
                this.Axis.Write();
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

            if (!Axis.isStatic)
            {
                builder.AppendLine(MakeHewName(Name + "_X_Axis", Axis.Address.ToHexString0x()));
                builder.AppendLine(MakeHEWReferenceScript(Address.ToHexString0x(), Name, 8));
            }

            return builder.ToString();
        }
    }

    public class RamTable2D : Table2D
    {

        public RamTable2D(XElement xel,Definition d, Table t)
            : base(xel,d,t)
        {

        }

    }
}
