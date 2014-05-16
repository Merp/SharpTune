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

    public class Table1D : Table
    {


        public Table1D(XElement xel, Definition def, Table basetable)
            : base(xel, def, basetable)
        { this.type = "1D"; }

        public override Table CreateChild(Lut lut,Definition d)
        {
            return base.CreateChild(lut,d);
        }

        public override void Read()
        {
            DeviceImage image = this.parentImage;

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
        public RamTable1D(XElement xel, Definition def, Table basetable)// DeviceImage image)
            : base(xel, def, basetable)
        {

        }

        public override void Read()
        {


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
