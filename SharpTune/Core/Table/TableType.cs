// TableType.cs: Table type enumeration, Subaru ROM specific.

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


namespace Subaru.Tables
{
	/// <summary>
	/// Actually ROM code cares about first byte only. Residual 3 bytes should be zero.
	/// Using full int32 here for convenience, improves speed, less code...
	/// </summary>
	public enum TableType
	{
		Float = 0x00,
		UInt8 = 0x04,
		UInt16 = 0x08,
		Int8 = 0x0C,
		Int16 = 0x10
	}
}
