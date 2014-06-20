// IdcType.cs: Enum containing type constants for IDC scripts.

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

namespace IDA
{
	[Flags]
	public enum IdcType : uint
	{
		/// <summary>
		/// 0x00000000 (!)
		/// </summary>
		FF_BYTE = 0x00000000,
		FF_WORD = 0x10000000,
		// dword
		FF_DWRD = 0x20000000,
		FF_QWRD = 0x30000000,
		FF_TBYT = 0x40000000,
		FF_ASCI = 0x50000000,
		FF_STRU = 0x60000000,
		/// <summary>
		// octaword (16 bytes)
		/// </summary>
		FF_OWRD = 0x70000000,
		FF_FLOAT = 0x80000000,
		FF_DOUBLE = 0x90000000,

		/// <summary>
		/// Hexadecimal number
		/// </summary>
		FF_0NUMH = 0x00100000,
		/// <summary>
		/// Decimal number
		/// </summary>
		FF_0NUMD = 0x00200000
	}
}
