using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.Core.Table
{
    public class RamTableReader : ITableReader
    {
        ///OLD CODE EXTRACTED FROM TABLE CLASSES
        ///
        //public override void Read()
        //{
        //    DeviceImage image = this.parentImage;
        //    this.elements = 1;
        //    Scaling sc;
        //    if (parentDef.ScalingList.TryGetValue(this.properties["scaling"].ToString(), out sc))
        //    {
        //        this.defaultScaling = sc;
        //    }
        //    else
        //        throw new Exception(String.Format("Error, scaling {0} not found!", this.properties["scaling"].ToString()));

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
        //}

        //public override void Read()
        //{


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
        //}
    }
}
