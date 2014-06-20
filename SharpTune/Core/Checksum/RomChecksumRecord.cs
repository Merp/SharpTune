// RomChecksumRecord.cs: Checksum record struct, Subaru ROM specific.

/* Copyright (C) 2011 SubaruDieselCrew
 *
 * This file is part of ScoobyRom.
 *
 * ScoobyRom is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ScoobyRom is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with ScoobyRom.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;

namespace SharpTune.Core.Checksum
{
	public struct RomChecksumRecord
	{
		int startAddress, endAddress, checksum;

		public RomChecksumRecord (int startAddress, int endAddress, int checksum) : this(startAddress, endAddress)
		{
			this.Checksum = checksum;
		}

		public RomChecksumRecord (int startAddresse, int endAddress)
		{
			this.startAddress = startAddresse;
			this.endAddress = endAddress;
			checksum = 0;
		}

		public int StartAddress {
			get { return startAddress; }
			private set {
				if (value % 4 != 0)
					throw new ArgumentException ("Start address must be at 32 bit alignment");
				this.startAddress = value;
			}
		}

		/// <summary>
		/// Address of last included byte.
		/// Zero is allowed designating empty block.
		/// </summary>
		public int EndAddress {
			get { return endAddress; }
			private set {
				if (value != 0 && value % 4 != 3)
					throw new ArgumentException ("End address must be one byte less than 32 bit alignment");
				this.endAddress = value;
			}
		}

		public int Checksum {
			get { return checksum; }
			set { checksum = value; }
		}

		/// <summary>
		/// startAddress == 0 && endAddress == 0
		/// </summary>
		public bool IsEmpty {
			get { return startAddress == 0 && endAddress == 0; }
		}

		public int BlockSize {
			get { return EndAddress - StartAddress + 1; }
		}

		public override string ToString ()
		{
			return string.Format ("[RomChecksumRecord: StartAddress={0:X}, EndAddress={1:X}, Checksum={2:X}]", StartAddress, EndAddress, Checksum);
		}
	}
}
