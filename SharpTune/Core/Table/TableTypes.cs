// TableTypes.cs: Table type parsing and conversion.

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
using System.Linq;
using IDA;
using Subaru.Tables;

namespace Subaru.Tables
{
	public static class TableTypes
	{
		// avoids having to use reflection for parsing, ToString() etc.
		static readonly Dictionary<TableType, string> tableTypeDict = new Dictionary<TableType, string> (6);

		static TableTypes ()
		{
			tableTypeDict.Add (TableType.Float, "Float");
			tableTypeDict.Add (TableType.UInt8, "UInt8");
			tableTypeDict.Add (TableType.UInt16, "UInt16");
			tableTypeDict.Add (TableType.Int8, "Int8");
			tableTypeDict.Add (TableType.Int16, "Int16");
		}

		public static string ToStr (this TableType tableType)
		{
			return tableTypeDict[tableType];
		}

		public static string[] GetStrings ()
		{
			return tableTypeDict.Values.ToArray ();
		}

		public static bool TryParse (string s, out TableType result)
		{
			result = 0;
			foreach (var item in tableTypeDict) {
				if (string.Equals (s, item.Value, StringComparison.InvariantCultureIgnoreCase)) {
					result = item.Key;
					return true;
				}
			}
			return false;
		}

		public static string ToRRType (this TableType tableType)
		{
			string s = tableType.ToStr ().ToLowerInvariant ();
			return s;
		}

		public static IdcType ToIdcType (this TableType tableType)
		{
			switch (tableType) {
			case TableType.Float:
				return IdcType.FF_FLOAT;
			case TableType.UInt8:
			case TableType.Int8:
				return IdcType.FF_BYTE;
			case TableType.UInt16:
			case TableType.Int16:
				return IdcType.FF_WORD;
			default:
				// unknown
				return 0;
			}
		}
	}
}
