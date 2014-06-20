using SharpTune.Core.MemoryModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpTune.Core.Checksum
{
    [Serializable]
    public class ChecksumSubaruDBW : IChecksumModule
    {
        public string Name
        {
            get { return "subarudbw"; }
        }

        public const int ChecksumTableRecordCount = 17;
		public const int ChecksumConstant = 0x5AA5A55A;
        IChecksumModule checksumModule;
		int tablePos;
		System.IO.Stream stream;
		List<RomChecksumRecord> checksumRecords;

		public ChecksumSubaruDBW (IMemoryModel memmodel, System.IO.Stream stream)//TODO: decouple these two
		{
            this.tablePos = memmodel.checksumtable;
            this.stream = stream;
		}

		public IList<RomChecksumRecord> ReadTableRecords ()
		{
			return ReadTableRecords (this.tablePos, ChecksumTableRecordCount);
		}

		public IList<RomChecksumRecord> ReadTableRecords (int count)
		{
			return ReadTableRecords (this.tablePos, count);
		}

		public IList<RomChecksumRecord> ReadTableRecords (int pos, int count)
		{
			List<RomChecksumRecord> records = new List<RomChecksumRecord> (count);

			stream.Seek (tablePos, System.IO.SeekOrigin.Begin);
			for (int i = 0; i < count; i++) {
				RomChecksumRecord record = new RomChecksumRecord (stream.ReadInt32BigEndian(), stream.ReadInt32BigEndian(), stream.ReadInt32BigEndian());
				records.Add (record);
			}
			this.checksumRecords = records;
			return records;
		}

		public int CalcSum (RomChecksumRecord record)
		{
			int sum = 0;
			if (!record.IsEmpty) {
				stream.Seek (record.StartAddress, SeekOrigin.Begin);
				for (int p = record.StartAddress; p < record.EndAddress; p += 4) {
					sum += stream.ReadInt32BigEndian ();
				}
			}
			return sum;
		}

		public int CalcChecksumValue (RomChecksumRecord record)
		{
			return ChecksumConstant - CalcSum (record);
		}

		public bool IsValid (RomChecksumRecord record)
		{
			return CalcChecksumValue (record) == record.Checksum;
		}

		/// <summary>
		/// Calculates Calibration Verification Number of 8-byte-type..
		/// This type of CVN is being used on diesel models.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int64"/>.
		/// To get CVN standard string "XXXXXXXX XXXXXXXX" use .ToString ("X16").Insert (8, " ");
		/// </returns>
		public long CalcCVN8 ()
		{
			// Could derive #3 from record.Checksum: sum3 = 0x5AA5A55A - record.Checksum
			// Must not extend as signed for last line to work correctly.
			ulong sum3 = (ulong)(uint)CalcSum (checksumRecords[3]);

			// #4: Actual sum needed, record.Checksum is different!
			ulong sum4 = (ulong)(uint)CalcSum (checksumRecords[4]);
			//Console.WriteLine ("sum3: {0:X8} sum4: {1:X8} sum3<<32: {2:X8}", sum3, sum4, sum3 << 32);

			// Use return type long (CLS-compliant) instead of ulong as it doesn't matter.
			return (long)((sum3 << 32) | sum4);
		}

		/// <summary>
		/// Makes CVN standard string "XXXXXXXX XXXXXXXX" out of given value.
		/// </summary>
		/// <param name="cvn">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public static string CVN8Str (long cvn)
		{
			return cvn.ToString ("X16").Insert (8, " ");
		}
    }
}
