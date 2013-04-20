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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.RomMod
{
    public class SRecordWriter
    {
        private TextWriter writer;

        public SRecordWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(uint startAddress, byte[] data)
        {
            int bytesWritten = 0;
            while (bytesWritten < data.Length)
            {
                byte count = (byte)Math.Min(16, data.Length - bytesWritten);

                // Add 5 to the count since the 4 address bytes and checksum will be written separately
                int checksum = count + 5;

                long address = startAddress + bytesWritten;
                SRecordWriter.CalculateChecksum(address, ref checksum);

                StringBuilder builder = new StringBuilder();

                for (int index = 0; index < count; index++)
                {
                    byte value = data[index + bytesWritten];
                    checksum += value;
                    builder.AppendFormat("{0:X2}", value);
                }

                byte lastByte = (byte)(checksum & 0xFF);
                byte complement = (byte)(lastByte ^ 0xFF);
                builder.Append(complement.ToString("X2"));

                string dataString = builder.ToString();

                // Adding 5 to the count since the 4 address bytes and checksum are written separately
                //Trace.WriteLine("S3{0:X2}{1:X8}{2}", count + 5, address, dataString);
                writer.WriteLine("S3{0:X2}{1:X8}{2}", count + 5, address, dataString);

                bytesWritten += (int)count;
            }
        }

        /// <summary>
        /// Calculate an SRecord checksum for the given 4-byte address;
        /// </summary>
        private static void CalculateChecksum(long address, ref int checksum)
        {
            byte b;
            b = (byte)((address & 0xFF000000) >> 24);
            checksum += b;
            b = (byte)((address & 0xFF0000) >> 16);
            checksum += b;
            b = (byte)((address & 0xFF00) >> 8);
            checksum += b;
            b = (byte)((address & 0xFF));
            checksum += b;
        }
    }
}
