using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomModCore;

namespace SharpTune.RomMod
{
    public class Lut
    {
        public byte[] rawdata;
        public int length;
        public byte cols;
        public byte rows;
        public uint colsAddress;
        public uint rowsAddress;
        public uint dataAddress;
        public uint tableType;
        public uint gradient;
        public uint offset;
        
        //for lookup tables
        public Lut()
        {
        }

        /// <summary>
        /// WARNING ONLY USE FOR 3d TABLES
        /// TODO arrange classes as 2D/3D and base
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="address"></param>

        public Lut(Blob blob, uint address)
        {
            int addr = (int)(address - (uint)blob.StartAddress);
            byte tb = 0;
            addr += 1;
            blob.TryGetByte(ref tb, ref addr);    //TODO CHANGE TO READ SHORTS!!!
            cols = tb;
            addr += 1;
            blob.TryGetByte(ref tb, ref addr);
            rows = tb;
            blob.TryGetUInt32(ref colsAddress, ref addr);
            blob.TryGetUInt32(ref rowsAddress, ref addr);
            blob.TryGetUInt32(ref dataAddress, ref addr);
            blob.TryGetUInt32(ref tableType, ref addr);
            blob.TryGetUInt32(ref gradient, ref addr);
            blob.TryGetUInt32(ref offset, ref addr);
        }
    }
}
