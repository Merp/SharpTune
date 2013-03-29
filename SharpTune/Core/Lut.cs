using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpTune.RomMod;

namespace SharpTune.Core
{
    public class Lut
    {
        public uint dataAddress;
        
        public Lut()
        {    
        }

        public Lut(uint da)
        {
            dataAddress = da;
        }
    }

    public class Lut3D : Lut2D
    {
        //public byte[] rawdata;
        //public int length;
        //public UInt16 cols;
        public UInt16 rows;
        //public uint colsAddress;
        public uint rowsAddress;
        //public uint dataAddress;
        //public uint tableType;
        //public uint gradient;
        //public uint offset;
        
        //for lookup tables
        public Lut3D()
        {
        }

        /// <summary>
        /// WARNING ONLY USE FOR 3d TABLES
        /// TODO arrange classes as 2D/3D and base
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="address"></param>

        public Lut3D(Blob blob, uint address)
        {
            int addr = (int)(address - (uint)blob.StartAddress);
            blob.TryGetUInt16(ref cols, ref addr);
            blob.TryGetUInt16(ref rows, ref addr);
            blob.TryGetUInt32(ref colsAddress, ref addr);
            blob.TryGetUInt32(ref rowsAddress, ref addr);
            blob.TryGetUInt32(ref dataAddress, ref addr);
            blob.TryGetUInt32(ref tableType, ref addr);
            blob.TryGetUInt32(ref gradient, ref addr);
            blob.TryGetUInt32(ref offset, ref addr);
        }
    }

    public class Lut2D : Lut
    {
        public byte[] rawdata;
        public int length;
        public UInt16 cols;
        public uint colsAddress;
        //public uint dataAddress;
        public uint tableType;
        public uint gradient;
        public uint offset;

        //for lookup tables
        public Lut2D()
        {
        }

        /// <summary>
        /// WARNING ONLY USE FOR 3d TABLES
        /// TODO arrange classes as 2D/3D and base
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="address"></param>

        public Lut2D(Blob blob, uint address)
        {
            int addr = (int)(address - (uint)blob.StartAddress);
            blob.TryGetUInt16(ref cols, ref addr);
            blob.TryGetUInt32(ref colsAddress, ref addr);
            blob.TryGetUInt32(ref dataAddress, ref addr);
            blob.TryGetUInt32(ref tableType, ref addr);
            blob.TryGetUInt32(ref gradient, ref addr);
            blob.TryGetUInt32(ref offset, ref addr);
        }
    }
}
