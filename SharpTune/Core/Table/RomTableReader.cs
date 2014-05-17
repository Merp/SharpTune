using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.Core.Table
{
    public class RomTableReader : ITableReader
    {
        ///OLD CODE EXTRACTED FROM TABLE CLASSES
        ///
        ////From table1d:
        //public override void Read()
        //{
        //    DeviceImage image = this.parentImage;

        //    lock (image.imageStream)
        //    {
        //        image.imageStream.Seek(this.address, SeekOrigin.Begin);
        //        this.byteValues = new List<byte[]>();
        //        this.displayValues = new List<string>();

        //        byte[] b = new byte[this.scaling.storageSize];
        //        image.imageStream.Read(b, 0, this.scaling.storageSize);
        //        if (this.scaling.endian == "big")
        //        {
        //            b.ReverseBytes();
        //        }
        //        this.byteValues.Add(b);
        //        this.displayValues.Add(this.scaling.toDisplay(b));

        //    }
        //}

        ////table2d:
        //public override void Read()
        //{
        //    DeviceImage image = this.parentImage;
        //    this.elements = this.yAxis.elements;    // * this.yAxis.elements;TODO WTF

        //    lock (image.imageStream)
        //    {
        //        image.imageStream.Seek(this.address, SeekOrigin.Begin);
        //        this.byteValues = new List<byte[]>();
        //        this.floatValues = new List<float>();
        //        this.displayValues = new List<string>();

        //        for (int i = 0; i < this.elements; i++)
        //        {
        //            byte[] b = new byte[this.scaling.storageSize];
        //            image.imageStream.Read(b, 0, this.scaling.storageSize);
        //            if (this.scaling.endian == "big")
        //            {
        //                b.ReverseBytes();
        //            }
        //            this.byteValues.Add(b);
        //            this.displayValues.Add(this.scaling.toDisplay(b));
        //        }

        //    }
        //}

        ////table3d
        ///// <summary>
        ///// Read table bytes from ROM
        ///// And convert to display values
        ///// </summary>
        //public override void Read()
        //{
        //    DeviceImage image = this.parentImage;
        //    this.elements = this.xAxis.elements * this.yAxis.elements;

        //    lock (image.imageStream)
        //    {
        //        image.imageStream.Seek(this.address, SeekOrigin.Begin);
        //        this.byteValues = new List<byte[]>();
        //        this.floatValues = new List<float>();
        //        this.displayValues = new List<string>();

        //        for (int i = 0; i < this.elements; i++)
        //        {
        //            byte[] b = new byte[this.scaling.storageSize];
        //            image.imageStream.Read(b, 0, this.scaling.storageSize);
        //            if (this.scaling.endian == "big")
        //            {
        //                b.ReverseBytes();
        //            }
        //            this.byteValues.Add(b);
        //            this.displayValues.Add(this.scaling.toDisplay(b));
        //        }

        //    }
        //}
    }
}
