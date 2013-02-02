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
    /// Merges the content of contiguous SRecords into a list of Blobs.
    /// </summary>
    public class BlobList
    {
        /// <summary>
        /// Congealed blobs.
        /// </summary>
        public readonly List<Blob> Blobs;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BlobList()
        {
            this.Blobs = new List<Blob>();
        }

        /// <summary>
        /// Add the payload of an SRecord to the appropriate blob, or create a new blob if necessary.
        /// </summary>
        public void ProcessRecord(SRecord record)
        {
            if (record.Payload == null)
            {
                return;
            }

            foreach (Blob blob in this.Blobs)
            {
                if (blob.NextByteAddress == record.Address)
                {
                    blob.AddRecord(record.Payload);
                    return;
                }
            }

            Blob newBlob = new Blob(record.Address, record.Payload);
            this.Blobs.Add(newBlob);
        }
    }
}
