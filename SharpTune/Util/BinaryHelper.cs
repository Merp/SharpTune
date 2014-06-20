// BinaryHelper.cs: Value types out of bytes.

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
using System.Collections.Generic;

namespace SharpTune
{
	public static class BinaryHelper
	{
		public static short Int16BigEndian (IList<byte> bytes, int index)
		{
			if (index + 1 >= bytes.Count || index < 0)
				throw new ArgumentOutOfRangeException ();
			return (short)(bytes[index + 1] | bytes[index] << 8);
		}

		public static ushort UInt16BigEndian (IList<byte> bytes, int index)
		{
			return (ushort)Int16BigEndian (bytes, index);
		}

		public static int Int32BigEndian (IList<byte> bytes, int index)
		{
			if (index + 3 >= bytes.Count || index < 0)
				throw new ArgumentOutOfRangeException ();
			// return (bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24); // normal = little-endian
			return bytes[index + 3] | bytes[index + 2] << 8 | bytes[index + 1] << 16 | bytes[index] << 24;
		}

		public static uint UInt32BigEndian (IList<byte> bytes, int index)
		{
			return (uint)Int32BigEndian (bytes, index);
		}

		public static float SingleBigEndian (IList<byte> bytes, int index)
		{
			if (index + 3 >= bytes.Count || index < 0)
				throw new ArgumentOutOfRangeException ();
			byte[] reordered = new byte[4];
			reordered[0] = bytes[index + 3];
			reordered[1] = bytes[index + 2];
			reordered[2] = bytes[index + 1];
			reordered[3] = bytes[index];
			// BitConverter.ToSingle assumes machine (little endian) order
			return BitConverter.ToSingle(reordered, 0);
		}
	}
}