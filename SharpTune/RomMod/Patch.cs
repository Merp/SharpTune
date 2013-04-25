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

namespace SharpTune.RomMod
{

        
    /// <summary>
    /// Describes the start and end addresses of an ECU patch (but not the content).
    /// </summary>
    /// <remarks>
    /// Corresponds to the metadata in a patch file.  See sample HEW project for details.
    /// </remarks>
    [Serializable]
    public class Patch
    {
        /// <summary>
        /// Start address of the patch payload
        /// Defined in ecuhacks (HEW) memory addressing (0x0F00XXXX)
        /// </summary>
        public uint CopyStartAddress { get; set; }

        /// <summary>
        /// Start address of the patch (the first byte to overwrite).
        /// Defined in actual rom addressing
        /// </summary>
        public uint StartAddress { get; private set; }

        /// <summary>
        /// End address of the patch (the last byte to overwrite).
        /// Defined in actual rom addressing
        /// </summary>
        public uint EndAddress { get; private set; }

        /// <summary>
        /// Length of the patch.
        /// </summary>
        public uint Length { get { return (this.EndAddress+1) - this.StartAddress; } }

        public bool IsMetaChecked { get; set; }

        /// <summary>
        /// Flag for Data/Code patches
        /// Used to prevent overwrites of existing data
        /// And allow unpatching after map changes
        /// </summary>
        public bool IsNewPatch { get; set; }

        /// <summary>
        /// Flag for baseline checks while unpatching
        /// </summary>

        public Blob Baseline { get; set; }

        public Blob Payload { get; set; }

        public String Name { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Patch(string name, uint start, uint end)
        {
            this.Name = name;
            this.StartAddress = start;
            this.EndAddress = end;
            this.IsNewPatch = false;
            this.IsMetaChecked = false;

        }

        /// <summary>
        /// Describe the patch range in human terms.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Patch name: " + this.Name + Environment.NewLine);
            sb.AppendLine(String.Format(" start: {0:X8}, end: {1:X8}, length: {2:X8}", this.StartAddress, this.EndAddress, this.Length));
            if(this.Length < 9)
                sb.AppendLine(String.Format(" baseline: {0:X}, payload: {1:X}",this.Baseline.GetDataString(),this.Payload.GetDataString()));
            return sb.ToString();
        }

        /// <summary>
        /// Check the baseline data for a r4b patch
        /// </summary>
        /// <param name="buffer">Buffer containing baseline data</param>
        /// <returns></returns>
        public virtual bool MetaCheck(IEnumerable<byte> buffer)
            {
            Blob checkblob = new Blob(this.StartAddress, buffer);

            int count = checkblob.Content.Count;

            for (int i = 0; i < count; i++)
            {
                if (this.Baseline.Content[i] != checkblob.Content[i])
                {
                    return false;
                }

            }
            return true;
        }

    }

    public class CalidPatch : Patch
    {
        public CalidPatch(string name, uint start, uint end)
            : base(name, start, end)
        {
        }
    }

    public class PullJSRHookPatch : Patch
    {
        public PullJSRHookPatch(string name, uint start, uint end)
            : base(name, start, end)
        {
            this.IsMetaChecked = true;
        }

        public override bool MetaCheck(IEnumerable<byte> buffer)
        {
            Blob checkblob = new Blob(this.StartAddress, buffer);

            int count = checkblob.Content.Count;
            byte[] mask = new byte[] { 0xF0, 0xFF, 0x00, 0x00 };
            byte[] check = new byte[] { 0x40, 0x0B, 0x00, 0x00 };
            byte[] baseline = buffer.ToArray<byte>();

            for (int i = 0; i < count; i++)
            {
                baseline[i] &= (byte)mask[i];
                if (check[i] != baseline[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
