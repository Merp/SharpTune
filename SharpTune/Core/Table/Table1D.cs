﻿/*
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
using System.Xml.XPath;
using System.Data;
using System.IO;
using SharpTune;
using System.Windows.Forms;
using System.Drawing;
using SharpTune.Core;
using System.Runtime.Serialization;
using SharpTune;

namespace SharpTuneCore
{

    public class Table1D : Table
    {
        public Table1D()
        { }

        public Table1D(XElement xel, Definition d, Table t)
            : base(xel, d, t)
        {
            this.ReadXmlECUFlash();
        }

        public override Table ConstructChild(XElement xel, Definition d)
        {
            return new Table1D(xel, d, this);
        }
        public override Table CreateChild(Lut lut,Definition d)
        {
            return base.CreateChild(lut,d);
        }

        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = 1;
            this.defaultScaling = SharpTuner.DataScalings.Find(s => s.name.ToString().Contains(this.properties["scaling"].ToString()));
            this.scaling = this.defaultScaling;

            lock (image.imageStream)
            {
                image.imageStream.Seek(this.Address, SeekOrigin.Begin);
                this.byteValues = new List<byte[]>();
                this.displayValues = new List<string>();

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

        public override void Write()
        {
            DeviceImage image = this.parentImage;
            lock (image.imageStream)
            {
                //No need to write axes, they don't really exist for 1D tables!
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
            return base.MakeHewName(Name, Address.ToHexString0x());
        }
    }


    public class RamTable1D : Table1D
    {
        protected string rrid;
        protected string rrtarget;
        protected List<Scaling> rrconversions;
        
        public RamTable1D(XElement xel, Definition d, Table t)// DeviceImage image)
            : base(xel, d, t)//MUDO: structure things so this ONLY initializes stuff!!!: 
        {
            
        }

        public RamTable1D()
        {
            // TODO: Complete member initialization
        }

        /// <summary>
        /// ONLY FOR ROMRAIDER CONVERSION
        /// </summary>
        /// <param name="xel"></param>
        public RamTable1D(XElement xel)
        {
            IsBase = true;
            name = xel.Attribute("name").Value.ToString();
            rrid = xel.Attribute("id").Value.ToString();
            description = xel.Attribute("desc").Value.ToString();
            rrtarget = xel.Attribute("target").Value.ToString();

            IEnumerable<XElement> xconversions = xel.XPathSelectElements("/ecuparam/conversions/conversion");
            foreach (XElement conv in xconversions)
            {
                //do something with the scalings.
            }
        }

        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = 1;
            this.defaultScaling = SharpTuner.DataScalings.Find(s => s.name.ToString().Contains(this.properties["scaling"].ToString()));
            this.scaling = this.defaultScaling;

            ////Check SSM interface ID vs the device ID
            //if (SharpTuner.ssmInterface.EcuIdentifier != this.parentImage.CalId)
            //{
            //    throw new System.Exception("Device Image does not match connected device!");
            //}

            //SsmInterface ssmInterface = SharpTuner.ssmInterface;

            //May have an issue with this while logging???
            //Is it necessary??
            //TODO: Find out
            //lock (ssmInterface)
            //{
            //    this.byteValues = new List<byte[]>();
            //    this.displayValues = new List<string>();

            //    byte[] b = new byte[this.scaling.storageSize];
            //    IAsyncResult result = ssmInterface.BeginBlockRead(this.address, this.scaling.storageSize, null, null);
            //    result.AsyncWaitHandle.WaitOne();
            //    b = ssmInterface.EndBlockRead(result);
            //    if (this.scaling.endian == "big")
            //    {
            //        b.ReverseBytes();
            //    }
            //    this.byteValues.Add(b);
            //    this.displayValues.Add(this.scaling.toDisplay(b));
            //}
        }
    }
}
