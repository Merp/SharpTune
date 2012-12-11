///*
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//*/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml.Linq;
//using System.Data;
//using System.IO;
//using Merp;
//using System.Windows.Forms;
//using System.Drawing;

//namespace SharpTune.Tables
//{
//    public class Table2D : Table
//    {

//        public Table2D(XElement xel, DeviceImage image)
//            : base(xel, image)
//        {

//        }

//        public override Table MergeTables(Table basetable)
//        {
//            foreach (KeyValuePair<string, string> property in basetable.properties)
//            {
//                //If property doesn't exist in the child, add it from the base!
//                if (!this.properties.ContainsKey(property.Key))
//                {
//                    this.properties.Add(property.Key, property.Value);
//                }
//            }

//            return this;
//        }

        

        
    
        


//        public override void Read()
//        {
//            DeviceImage image = this.parentImage;
//            this.elements = this.yAxis.elements;    // * this.yAxis.elements;
//            this.defaultScaling = image.Definition.scalingList.Find(s => s.name.ToString().Contains(this.properties["scaling"].ToString()));
//            this.scaling = this.defaultScaling;

//            lock (image.imageStream)
//            {
//                image.imageStream.Seek(this.address, SeekOrigin.Begin);
//                this.byteValues = new List<byte[]>();
//                this.floatValues = new List<float>();
//                this.displayValues = new List<string>();

//                for (int i = 0; i < this.elements; i++)
//                {
//                    byte[] b = new byte[this.scaling.storageSize];
//                    image.imageStream.Read(b, 0, this.scaling.storageSize);
//                    if (this.scaling.endian == "big")
//                    {
//                        b.ReverseBytes();
//                    }
//                    this.byteValues.Add(b);
//                    this.displayValues.Add(this.scaling.toDisplay(b));
//                }

//            }
//        }

//        public override void Write()
//        {
//            DeviceImage image = this.parentImage;
//            lock (image.imageStream)
//            {
//                //2D only has Y axis
//                this.yAxis.Write();
//                image.imageStream.Seek(this.address, SeekOrigin.Begin);

//                //write this.bytevalues!
//                foreach (byte[] bytevalue in this.byteValues)
//                {
//                    if (this.scaling.endian == "big")
//                    {
//                        bytevalue.ReverseBytes();
//                    }
//                    image.imageStream.Write(bytevalue, 0, bytevalue.Length);
//                }
//            }

//        }

//    }

//    public class RamTable2D : Table2D
//    {

//        public RamTable2D(XElement xel, DeviceImage image)
//            : base(xel, image)
//        {

//        }

//    }
//}
