using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.Core.Table
{
    public class RomTableWriter : ITableWriter
    {

        ///OLD CODE EXTRACTED FROM TABLE CLASSES
        ///
    //    //from table1d
    //    public override void Write()
    //    {
    //        DeviceImage image = this.parentImage;
    //        lock (image.imageStream)
    //        {
    //            //No need to write axes, they don't really exist for 1D tables!
    //            image.imageStream.Seek(this.address, SeekOrigin.Begin);

    //            //write this.bytevalues!
    //            foreach (byte[] bytevalue in this.byteValues)
    //            {
    //                if (this.scaling.endian == "big")
    //                {
    //                    bytevalue.ReverseBytes();
    //                }
    //                image.imageStream.Write(bytevalue, 0, bytevalue.Length);
    //            }
    //        }

    //    }

    //    //table2d:
        

    //    public override void Write()
    //    {
    //        DeviceImage image = this.parentImage;
    //        lock (image.imageStream)
    //        {
    //            //2D only has Y axis
    //            this.yAxis.Write();
    //            image.imageStream.Seek(this.address, SeekOrigin.Begin);

    //            //write this.bytevalues!
    //            foreach (byte[] bytevalue in this.byteValues)
    //            {
    //                if (this.scaling.endian == "big")
    //                {
    //                    bytevalue.ReverseBytes();
    //                }
    //                image.imageStream.Write(bytevalue, 0, bytevalue.Length);
    //            }
    //        }

    //    }

        
    //    //table3d
        

    //    /// <summary>
    //    /// Write the table bytes to ROM
    //    /// </summary>
    //    public override void Write()
    //    {
    //        DeviceImage image = this.parentImage;

    //        lock (image.imageStream)
    //        {
    //            //2D only has Y axis
    //            this.yAxis.Write();
    //            this.xAxis.Write();
    //            image.imageStream.Seek(this.address, SeekOrigin.Begin);

    //            //write this.bytevalues!
    //            foreach (byte[] bytevalue in this.byteValues)
    //            {
    //                if (this.scaling.endian == "big")
    //                {
    //                    bytevalue.ReverseBytes();
    //                }
    //                image.imageStream.Write(bytevalue, 0, bytevalue.Length);
    //            }
    //        }

    //    }
    //}
    }
}
