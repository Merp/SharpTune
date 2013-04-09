/*
 * This code is derived from the Java version of RomRaider
 *
 * RomRaider Open-Source Tuning, Logging and Reflashing
 * Copyright (C) 2006-2012 RomRaider.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.Collections.Generic;
using RomRaider.Logger.External.Aem.Xwifi.Plugin;
using Sharpen;

namespace RomRaider.Logger.External.Aem.Xwifi.Plugin
{
	/// <summary>
	/// Enum class representing the AEM X-Wifi sensor types available with methods to
	/// translate the mnemonic and numerical values.
	/// </summary>
	/// <remarks>
	/// Enum class representing the AEM X-Wifi sensor types available with methods to
	/// translate the mnemonic and numerical values.
	/// </remarks>
	public class AemSensor
	{
		public enum AemSensorType
		{
			UEGO,
			EGT1,
			EGT2,
			UNDEFINED
		}

		private static readonly IDictionary<int, AemSensor.AemSensorType> lookup = new Dictionary
			<int, AemSensor.AemSensorType>();

		static AemSensor()
		{
			// Returned when no match is found for get()
			//	    public int get() {
			//	        return value;
			//	    }
			foreach (AemSensor.AemSensorType s in EnumSet.AllOf<AemSensor.AemSensorType>())
			{
				lookup.Put(s.value, s);
			}
		}

		/// <summary>
		/// Get the <b>AemSensorType</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </summary>
		/// <remarks>
		/// Get the <b>AemSensorType</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </remarks>
		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>AemSensorType</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static AemSensor.AemSensorType ValueOf(int value)
		{
			if (lookup.ContainsKey(value))
			{
				return lookup.Get(value);
			}
			else
			{
				return AemSensor.AemSensorType.UNDEFINED;
			}
		}
	}
}
