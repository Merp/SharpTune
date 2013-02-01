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
using NateW.Ssm;
using Merp;
using System.Windows.Forms;
using System.Drawing;

namespace SharpTune.Tables
{
    public class Table1D : Table
    {


        public Table1D(XElement xel, DeviceImage image)
            : base(xel, image)
        {

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

        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = 1;
            this.defaultScaling = image.Definition.scalingList.Find(s => s.name.ToString().Contains(this.properties["scaling"].ToString()));
            this.scaling = this.defaultScaling;

            lock (image.imageStream)
            {
                image.imageStream.Seek(this.address, SeekOrigin.Begin);
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


    public class RamTable1D : Table1D
    {
        public RamTable1D(XElement xel, DeviceImage image)
            : base(xel, image)
        {

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

        public override void Read()
        {
            DeviceImage image = this.parentImage;
            this.elements = 1;
            this.defaultScaling = image.Definition.scalingList.Find(s => s.name.ToString().Contains(this.properties["scaling"].ToString()));
            this.scaling = this.defaultScaling;

            //Check SSM interface ID vs the device ID
            if(this.parentImage.parent.ssmInterface.EcuIdentifier != this.parentImage.CalId)
            {
                throw new System.Exception("Device Image does not match connected device!");
            }

            SsmInterface ssmInterface = this.parentImage.parent.ssmInterface;
            
            //May have an issue with this while logging???
            //Is it necessary??
            //TODO: Find out
            lock (ssmInterface)
            {
                this.byteValues = new List<byte[]>();
                this.displayValues = new List<string>();

                byte[] b = new byte[this.scaling.storageSize];
                IAsyncResult result = ssmInterface.BeginBlockRead(this.address, this.scaling.storageSize, null, null);
                result.AsyncWaitHandle.WaitOne();
                b = ssmInterface.EndBlockRead(result);
                if (this.scaling.endian == "big")
                {
                    b.ReverseBytes();
                }
                this.byteValues.Add(b);
                this.displayValues.Add(this.scaling.toDisplay(b));
            }
        }
    }
}
