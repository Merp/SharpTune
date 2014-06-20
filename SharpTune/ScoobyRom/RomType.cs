// RomType.cs: enum

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

namespace Subaru
{
	public enum RomType
	{
		Unknown,
		/// <summary>
		/// 512 KiB
		/// </summary>
		SH7055,
		/// <summary>
		/// 1 MiB
		/// </summary>
		SH7058,
		/// <summary>
		/// 1.5 MiB
		/// </summary>
		SH7059
	}
}
