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

using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class EcuSwitchConvertorImpl : EcuDataConvertor
	{
		private static readonly GaugeMinMax GAUGE_MIN_MAX = new GaugeMinMax(0.0, 1.0, 1.0
			);

		private static readonly string FORMAT = "0";

		private readonly int bit;

		public EcuSwitchConvertorImpl(int bit)
		{
			ParamChecker.CheckBit(bit);
			this.bit = bit;
		}

		public double Convert(byte[] bytes)
		{
			return (bytes[0] & (1 << bit)) > 0 ? 1 : 0;
		}

		public string GetUnits()
		{
			return "On/Off";
		}

		public GaugeMinMax GetGaugeMinMax()
		{
			return GAUGE_MIN_MAX;
		}

		public string GetFormat()
		{
			return FORMAT;
		}

		public string Format(double value)
		{
			//return value > 0 ? "On" : "Off";
			return value > 0 ? "1" : "0";
		}

		public override string ToString()
		{
			return GetUnits();
		}
	}
}
