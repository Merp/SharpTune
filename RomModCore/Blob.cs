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

namespace RomModCore
{
    /// <summary>
    /// Basically a list of bytes plus a start-address.
    /// </summary>
    public class Blob
    {
        /// <summary>
        /// Where to put these bytes.
        /// </summary>
        public uint StartAddress { get; private set; }
        
        /// <summary>
        /// The bytes.
        /// </summary>
        public List<byte> Content { get; private set; }

        /// <summary>
        /// Address of the next byte (following this blob).
        /// </summary>
        public uint NextByteAddress { get { return this.StartAddress + (uint) this.Content.Count; } }

        /// <summary>
        /// Create a new blob using the given start address and content.
        /// </summary>
        public Blob(uint startAddress, IEnumerable<byte> content)
        {
            this.StartAddress = startAddress;
            this.Content = new List<byte>();
            this.Content.AddRange(content);
        }

        public Blob CloneWithNewStartAddress(uint newStartAddress)
        {
            return new Blob(newStartAddress, this.Content);
        }

        /// <summary>
        /// Add the given content to the blob.
        /// </summary>
        /// <param name="record"></param>
        public void AddRecord(IEnumerable<byte> content)
        {

            this.Content.AddRange(content);
        }

        /// <summary>
        /// Describe the blob in human terms.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Start: {0:X8}, Length: {1:X8}", this.StartAddress, this.Content.Count);
        }

        /// <summary>
        /// Try to get a byte from the blob at the given offset.  If successful, increment the offset.
        /// </summary>
        public bool TryGetByte(ref byte result, ref int offset)
        {
            if (this.Content.Count > offset)
            {
                result = this.Content[offset];
                offset++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get an unsigned 32-bit integer from the blob at the given offset.  If successful, increment the offset.
        /// </summary>
        public bool TryGetUInt32(ref uint result, ref int offset)
        {
            if (this.Content.Count < offset + 4)
            {
                return false;
            }

            result = 0;
            byte temp = 0;
            if (!this.TryGetByte(ref temp, ref offset))
            {
                return false;
            }

            result |= temp;
            if (!this.TryGetByte(ref temp, ref offset))
            {
                return false;
            }

            result <<= 8;
            result |= temp;

            if (!this.TryGetByte(ref temp, ref offset))
            {
                return false;
            }

            result <<= 8;
            result |= temp;
            if (!this.TryGetByte(ref temp, ref offset))
            {
                return false;
            }

            result <<= 8;
            result |= temp;
            return true;
        }
    }
}
