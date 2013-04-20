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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpTune.RomMod
{
    class Verifier : IDisposable
    {
        private string patchPath;
        private string romPath;
        private SRecordReader patchReader;
        private Stream romStream;
        private bool apply;
        private bool dispose;

        public Verifier(string patchPath, string romPath, bool apply)
        {
            this.patchPath = patchPath;
            this.romPath = romPath;
            this.apply = apply;
            this.patchReader = new SRecordReader(patchPath);
            this.romStream = File.OpenRead(romPath);
            this.dispose = true;
        }

        public Verifier(Stream rs, SRecordReader sr, bool apply)
        {
            this.romStream = rs;
            this.patchReader = sr;
            this.apply = apply;
            this.dispose = false;
        }

        public void Dispose()
        {
            if (dispose)
            {
                if (this.patchReader != null)
                {
                    this.patchReader.Dispose();
                    this.patchReader = null;
                }

                if (this.romStream != null)
                {
                    this.romStream.Dispose();
                    this.romStream = null;
                }
            }
        }

        public bool TryVerify(IList<Patch> patches)
        {
            foreach (Patch patch in patches)
            {
                if (!this.TryVerifyPatch(patch))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryVerifyPatch(Patch patch)
        {
           // using (this.patchReader)
            //using (this.romStream)
            //{
              //  this.patchReader.Open();
                SRecord record;
                while (this.patchReader.TryReadNextRecord(out record))
                {
                    if (!this.TryProcessRecord(patch, record))
                    {
                        Dispose();
                        return false;
                    }
                }

                Dispose();
                return true;
                
        }

        private bool TryProcessRecord(Patch patch, SRecord record)
        {
            if (record.Payload == null)
            {
                return true;
            }

            int startAddress = (int)(this.apply ? patch.StartAddress : patch.StartAddress + Mod.BaselineOffset);
            int endAddress = (int)(this.apply ? patch.EndAddress : patch.EndAddress + Mod.BaselineOffset);

            for (uint address = record.Address; address < record.Address + record.Payload.Length; address++)
            {
                if (address >= startAddress && address <= endAddress)
                {
                    this.romStream.Position = address;
                    int i = this.romStream.ReadByte();
                    if (i == -1)
                    {
                        Trace.WriteLine(String.Format("Reached end of file while trying to verify address {0:X8}", address));
                        return false;
                    }

                    int recordAddresss = (int)(this.apply ? record.Address : record.Address - Mod.BaselineOffset);

                    uint payloadIndex = address - record.Address;
                    byte b = record.Payload[payloadIndex];
                    if (i != b)
                    {
                        Trace.WriteLine(String.Format("Address {0:X8} contains {1:X2}, but should contain {2:X2}", address, b, payloadIndex));
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
